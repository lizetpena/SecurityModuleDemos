using ContosoCorp.Demo1.Complete.Data;
using ContosoCorp.Demo1.Complete.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ContosoCorp.Demo1.Complete.Extensions
{
    public static class UserInitializer
    {
        public static async Task SeedUsers(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Ensure our database has been created/update
            context.Database.Migrate();

            if (userManager.Users.Any())
            {
                return;
            }

            var users = new ApplicationUser[]
            {
                new ApplicationUser { Email = "suzyq@contosocorp.com", UserName = "suzyq@contosocorp.com" },
                new ApplicationUser { Email = "johndoe@contosocorp.com", UserName = "johndoe@contosocorp.com" },
                new ApplicationUser { Email = "janedoe@contosocorp.com", UserName = "janedoe@contosocorp.com" }
            };

            foreach (var user in users)
            {
                // Create our users with an *awesome* password
                await userManager.CreateAsync(user, "Passw0rd!");
            }

            var suzy = await userManager.FindByEmailAsync("suzyq@contosocorp.com");
            var john = await userManager.FindByEmailAsync("johndoe@contosocorp.com");
            var jane = await userManager.FindByEmailAsync("janedoe@contosocorp.com");

            // Suzy was born in 1970 and is a founder!
            await userManager.AddClaimsAsync(suzy, new List<Claim>
            {
                new Claim("DateOfBirth", new DateTime(1970, 1, 1).ToString()),
                new Claim("EmployeeNumber", "1")
            });

            // John was born in 1985 and is a founder!
            await userManager.AddClaimsAsync(john, new List<Claim>
            {
                new Claim("DateOfBirth", new DateTime(1985, 1, 1).ToString()),
                new Claim("EmployeeNumber", "4")
            });

            // Jane 20 years old from now and is a regular employee!
            await userManager.AddClaimsAsync(jane, new List<Claim>
            {
                new Claim("DateOfBirth", DateTime.Today.AddYears(-20).ToString()),
                new Claim("EmployeeNumber", "101")
            });
        }
    }
}
