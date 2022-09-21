using E_Learning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Logging;
using Microsoft.Extensions.Logging;

namespace E_Learning.Seeds
{
    public class SeedUsersData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<SeedUsersData>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roles = { "Admin", "Student", "Teacher" };


            logger.LogInformation("Seeding users data...");
            
            foreach (string role in roles)
            {
                string email = $"{role.ToLower()}@example.com";
                string password = "Hello%world1";

                ApplicationUser user = new()
                {
                    UserName = email,
                    Email = email
                };

                var oldUser = await userManager.FindByEmailAsync(email);

                if (oldUser == null)
                {
                    IdentityResult result = userManager.CreateAsync(user, password).Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, role).Wait();
                    }

                    logger.LogInformation($"Initial {role.ToLower()} user created.");
                }
                else
                {
                    logger.LogInformation($"Default {role.ToLower()} user is already created.");
                }
            }
        }
    }
}
