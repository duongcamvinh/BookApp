using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace Api.Entities
{
    public class AppUserRole:IdentityUserRole<int>
    {
       public AppUsers Users { get; set; }
        public AppRole Role { get; set; }
    }
}
