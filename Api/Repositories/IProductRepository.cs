using Api.Models;
using Api.Paginations;

namespace Api.Repositories;

public interface IProductRepository : IRepository<ProductModel>
{
    Task<PagedList<ProductModel>> GetProductsPaginationAsync(ProductParameters productParameters);

    Task<IEnumerable<ProductModel>> GetProductsByNameAsync();
}
