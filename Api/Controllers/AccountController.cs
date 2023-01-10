using api.DTO;
using Api.Data;
using Api.DTO;
using Api.Entities;
using Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUsers> _userManager;
        private readonly SignInManager<AppUsers> _signInManager;
        // private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        public AccountController(IMapper mapper, IEmailSender emailSender, ITokenService tokenService, UserManager<AppUsers> userManager, SignInManager<AppUsers> signInManager, IConfiguration config)
        {

            _mapper = mapper;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _emailSender = emailSender;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.UserName.ToLower()))
                return BadRequest("UserName is already taken");
            var user = _mapper.Map<AppUsers>(registerDto);
            user.UserName = registerDto.UserName.ToLower();
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            await _userManager.AddToRoleAsync(user, "User");
            
            if (result.Succeeded)
            {
                var userFromDb = await _userManager.FindByNameAsync(user.UserName);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(userFromDb);
                var uriBuilder = new UriBuilder(_config["ReturnPaths:ConfirmEmail"]!);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["token"] = token;
                query["userid"] = (user.Id).ToString();
                uriBuilder.Query = query.ToString();
                var urlString = uriBuilder.ToString();
                var senderEmail = _config["ReturnPaths:SenderEmail"];
                await _emailSender.SendEmailAsync(senderEmail, user.Email, "Confirm your email adress", urlString);
                return new UserDTO
                {
                    UserName = user.UserName,
                    Token = await _tokenService.CreateToken(user)
                };            
            }
            return BadRequest("Register fail");
            //var user = new AppUsers
            //{
            //    UserName = registerDto.UserName.ToLower(),
            //};


        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);
            if (user == null)
            {
                return BadRequest("Invalid UserName");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if (!result.Succeeded)
                return Unauthorized();
            return new UserDTO
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user)
            };

        }
        [HttpPost("renewtoken")]
        public async Task<ActionResult<TokenDto>> RenewToken(TokenDto token)
        {
            try
            {
                var result = await _tokenService.RenewToken(token);
                return result;
            }
            catch (Exception e)
            {
                var message = e.Message;
                return BadRequest(message);
            }

        }
        [HttpPost("confirmemail" )]
        public async Task<ActionResult> ConFirmEmail(ConfirmMailDto con)
        {

            var user = await _userManager.FindByIdAsync(con.UserId);
            var result = await _userManager.ConfirmEmailAsync(user, con.Token);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }
    }

}







