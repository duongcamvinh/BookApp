using Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.Data
{
    public class Seed
    {
        public static async Task SeedBook(DataContext context)
        {
            if (await context.Books.AnyAsync())
                return;
            var bookData = await System.IO.File.ReadAllTextAsync("Data/BookSeedData.json");
             var books= JsonSerializer.Deserialize<List<AppBooks>>(bookData);
            foreach(var book in books)
            {
                book.BookName = book.BookName.ToLower();
                context.Books.Add(book);
            }
            await context.SaveChangesAsync();

        }
        public static async Task SeedUser(UserManager<AppUsers> userManager,RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync())
                return;
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUsers>>(userData);
            if (users == null) return;
            var roles = new List<AppRole>
            {
                new AppRole{Name="Member"},
                new AppRole{Name="User"},
                new AppRole{Name="Admin"}
            };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach(var user in users)
            {
              
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "password");
                await userManager.AddToRoleAsync(user, "User");
            }
            var admin = new AppUsers
            {
                UserName = "admin"
            };
            await userManager.CreateAsync(admin, "password");
            await userManager.AddToRoleAsync(admin, "Admin");
            await userManager.AddToRoleAsync(admin, "Member");
            var member = new AppUsers
            {
                UserName = "member"
            };
            await userManager.CreateAsync(member, "password");
            await userManager.AddToRoleAsync(member, "Member");
        }

    }
}
