using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Api.Entities
{

    [Table("RefreshToken")]
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public AppUsers AppUsers { get; set; }
        public int AppUsersId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public DateTime Expired { get; set; }
        public DateTime IssuedAt { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoke { get; set; }
    }
}

