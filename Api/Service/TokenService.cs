using api.DTO;
using Api.Data;
using Api.Entities;
using Api.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Api.Service
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUsers> _userManager;
        private readonly DataContext _context;

        public TokenService(IConfiguration config, UserManager<AppUsers> userManager, DataContext context)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _userManager = userManager;
            _context = context;
        }



        public async Task<TokenDto> CreateToken(AppUsers user)
        {
            var claim = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.NameId, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.UniqueName,user.Id.ToString())
            };
            var roles = await _userManager.GetRolesAsync(user);
            claim.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.Now.AddMilliseconds(5), // thời gian hiệu lực của token
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            var refreshToken = GenerateToken();
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                AppUsersId = user.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoke = false,
                IssuedAt = DateTime.UtcNow,
                Expired = DateTime.UtcNow.AddHours(1)
            };
            await _context.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();
            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        public string GenerateToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }
            return Convert.ToBase64String(random);
        }

        public  async Task<TokenDto> RenewToken(TokenDto tokendto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidateParam = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
            //check 1 :AccessToken Validate Format
            var tokenInVerification = tokenHandler.ValidateToken(tokendto.AccessToken, tokenValidateParam, out var validateToken);
            //check 2 check alg
            if (validateToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                if (!result)//false
                {
                    return null;
                }
            }
            //check 3 :check Token expire?
            var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expireDate = ConverUnixTimeToDateTime(utcExpireDate);
            if (expireDate > DateTime.UtcNow)
            {
                return null;
            }
            //check 4 :check RefrestToken exist in DB
            var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == tokendto.RefreshToken);
            if (storedToken == null)
            {
                return null;
            }
            //check 5: check RefreshToken is used/Revoke?
            if (storedToken.IsUsed)
            {
                return null;
            }
            if (storedToken.IsRevoke)
            {
                return null;
            }
            //check 6 :AccesToken Id= Jwt in RefreshToken
            var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            if (storedToken.JwtId != jti)
            {
                return null;
            }
            //update token
            storedToken.IsUsed = true;
            storedToken.IsRevoke = true;
            _context.Update(storedToken);
            await _context.SaveChangesAsync();
            //create new  token
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == storedToken.AppUsersId);
            var token = await CreateToken(user);
            return token;
        }
        public DateTime ConverUnixTimeToDateTime(long utcExpireDate)
        {
            {
                var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
                return dateTimeInterval;
            }
        }
    }
}



