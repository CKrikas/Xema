using Microsoft.AspNetCore.Identity;
using xema.Models;

namespace xema.Data;

public static class DbInitializer
{
    public static async Task Seed(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        // Seed default categories
        var defaultCategories = new List<Category>
        {
            new() { Name = "Uncategorized" }
        };

        foreach (var category in defaultCategories)
        {
            if (!context.Categories.Any(c => c.Name == category.Name))
            {
                context.Categories.Add(category);
            }
        }

        await context.SaveChangesAsync();


        // Ensure Admin role exists
        const string adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult = await roleManager.CreateAsync(
                new IdentityRole(adminRole));

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    "Failed to create Admin role: " +
                    string.Join(", ",
                        roleResult.Errors.Select(e => e.Description)));
            }
        }


        // Get admin credentials from User Secrets / configuration
        var adminEmail = configuration["Admin:Email"];
        var adminPassword = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new Exception(
                "Admin credentials are not configured.");
        }

        // Find existing admin user
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        // Create admin user if it doesn't exist
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                adminUser,
                adminPassword);

            if (!result.Succeeded)
            {
                throw new Exception(
                    "Failed to create admin user: " +
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }
        }

        // Ensure admin user has Admin role
        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            var roleResult = await userManager.AddToRoleAsync(
                adminUser,
                adminRole);

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    "Failed to assign Admin role: " +
                    string.Join(", ",
                        roleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}