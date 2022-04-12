using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public class Seed
    {
       // public static async Task SeedUsers(DataContext context)
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
           // if(context.Users.Any()) return;
           if(userManager.Users.Any()) return;
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"}
                
            };

            foreach (var role in roles)
            {
               await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();

                //Commented to use Microsoft Identity feature
                /* using var hmac = new HMACSHA512();               
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
                user.PasswordSalt = hmac.Key; */

              //  context.Users.Add(user);
             await userManager.CreateAsync(user,"password");
             await userManager.AddToRoleAsync(user,"Member");

            }
            var admin = new AppUser{
                UserName = "adamin"
            };
            await userManager.CreateAsync(admin,"password");
            await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});

           // await context.SaveChangesAsync();
        }
    }
}