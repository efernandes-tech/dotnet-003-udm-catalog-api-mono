using Api.Context;
using Api.Models;
using Api.Paginations;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class CategoryRepository : Repository<CategoryModel>, ICategoryRepository
{
    public CategoryRepository(CatalogoApiDbContext context) : base(context)
    {
    }

    public async Task<PagedList<CategoryModel>> GetCategoriesPaginationAsync(CategoryParameters categoryParameters)
    {
        return await PagedList<CategoryModel>.ToPagedListAsync(
            Get().OrderBy(s => s.Name),
            categoryParameters.PageNumber,
            categoryParameters.PageSize
        );
    }

    public async Task<IEnumerable<CategoryModel>> GetCategoryWithProductsAsync()
    {
        return await Get().Include(x => x.Products).ToListAsync();
    }
}
