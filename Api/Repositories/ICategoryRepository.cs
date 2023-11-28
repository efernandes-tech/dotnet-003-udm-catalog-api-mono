using Api.Models;
using Api.Paginations;

namespace Api.Repositories;

public interface ICategoryRepository : IRepository<CategoryModel>
{
    Task<PagedList<CategoryModel>> GetCategoriesPaginationAsync(CategoryParameters categoryParameters);

    Task<IEnumerable<CategoryModel>> GetCategoryWithProductsAsync();
}
