using Api.Context;
using Api.Models;

namespace UnitTests;

public class DbUnitTestsMockInitializer
{
    public DbUnitTestsMockInitializer()
    { }

    public void Seed(CatalogoApiDbContext context)
    {
        context.Categories.Add(
            new CategoryModel
            {
                Id = 1,
                Name = "Category Test 1",
                Description = "Description Test 1",
            });
        context.Categories.Add(
            new CategoryModel
            {
                Id = 2,
                Name = "Category Test 2",
                Description = "Description Test 2",
            });
        context.Categories.Add(
            new CategoryModel
            {
                Id = 3,
                Name = "Category Test 3",
                Description = "Description Test 3",
            });
        context.SaveChanges();
    }
}
