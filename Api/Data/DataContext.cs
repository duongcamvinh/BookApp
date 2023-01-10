using Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class DataContext : IdentityDbContext<AppUsers, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions option) : base(option)
        {

        }
        public DbSet<AppBooks> Books { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        // public DbSet<AppUsers> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
                builder.Entity<AppUsers>()
                .HasMany(ur=> ur.UserRole)
                .WithOne(u=>u.Users)
                .HasForeignKey(ur=>ur.UserId)
                .IsRequired();
            builder.Entity<AppRole>()
                .HasMany(u => u.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .IsRequired();
        }
    }
}
