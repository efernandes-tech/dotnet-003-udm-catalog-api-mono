using Api.Context;
using Api.Models;

namespace Api.Data;

public class DbInitializer
{
    public static void SeedData(CatalogoApiDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Categories.Any())
        {
            var categories = new CategoryModel[]
            {
                new CategoryModel { Name = "Fruteira", CreatedAt = DateTime.UtcNow },
                new CategoryModel { Name = "Padaria", CreatedAt = DateTime.UtcNow },
                new CategoryModel { Name = "Papelaria", CreatedAt = DateTime.UtcNow },
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        if (!context.Products.Any())
        {
            var products = new ProductModel[]
            {
                new ProductModel{ Name = "Banana", Quantity = 1, CategoryId = 1 , CreatedAt = DateTime.UtcNow },
                new ProductModel{ Name = "Pao", Quantity = 1, CategoryId = 2 , CreatedAt = DateTime.UtcNow },
                new ProductModel{ Name = "Papel A4", Quantity = 1, CategoryId = 3 , CreatedAt = DateTime.UtcNow },
            };
            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}