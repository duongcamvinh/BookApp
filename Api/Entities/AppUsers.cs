using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Api.Entities
{
    public class AppUsers : IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }
        public string KnowAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<AppUserRole> UserRole { get; set; }
    }
}
