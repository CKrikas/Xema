using xema.Models;

namespace xema.Data;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        var defaultCategories = new List<Category>
        {
            new() { Name = "Anime" },
            new() { Name = "Blackwork" },
            new() { Name = "Fine Line" },
            new() { Name = "Realism" },
            new() { Name = "Traditional" }
        };

        foreach (var category in defaultCategories)
        {
            if (!context.Categories.Any(c => c.Name == category.Name))
            {
                context.Categories.Add(category);
            }
        }

        context.SaveChanges();
    }
}