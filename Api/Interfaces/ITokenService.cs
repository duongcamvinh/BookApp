using api.DTO;
using Api.Entities;
using System;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface ITokenService
    {
      Task<TokenDto> CreateToken(AppUsers user);
        public string GenerateToken();
        public Task<TokenDto> RenewToken(TokenDto tokendto);
        public DateTime ConverUnixTimeToDateTime(long utcExpireDate);
    }
}
