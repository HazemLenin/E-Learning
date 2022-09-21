using E_Learning.Data;
using Microsoft.AspNetCore.Identity;

namespace E_Learning.Seeds
{
    public class SeedRolesData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<SeedRolesData>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            logger.LogInformation("Seeding roles...");
            string[] roles = { "Student", "Teacher", "Admin" };
            int createdRolesCounter = 0;

            foreach (string role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    createdRolesCounter++;
                }
            }

            logger.LogInformation($"Created {createdRolesCounter} Roles.");
        }
    }
}
