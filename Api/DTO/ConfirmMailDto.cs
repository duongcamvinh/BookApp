using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class ConfirmMailDto
    {      
        public string Token { get; set; }     
        public string UserId { get; set; }
    }
}
