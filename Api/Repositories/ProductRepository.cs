using Api.Context;
using Api.Models;
using Api.Paginations;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class ProductRepository : Repository<ProductModel>, IProductRepository
{
    public ProductRepository(CatalogoApiDbContext context) : base(context)
    {
    }

    public async Task<PagedList<ProductModel>> GetProductsPaginationAsync(ProductParameters productParameters)
    {
        return await PagedList<ProductModel>.ToPagedListAsync(
            Get().OrderBy(s => s.Name),
            productParameters.PageNumber,
            productParameters.PageSize
        );
    }

    public async Task<IEnumerable<ProductModel>> GetProductsByNameAsync()
    {
        return await Get().OrderBy(x => x.Name).ToListAsync();
    }
}
