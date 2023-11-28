using Api.DTOs;
using Api.Models;
using Api.Paginations;
using Api.Repositories;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Controllers.V1;

[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _uofw;
    private readonly IMapper _mapper;

    public ProductsController(IUnitOfWork uofw, IMapper mapper)
    {
        _uofw = uofw;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAsync([FromQuery] ProductParameters productParameters)
    {
        var products = await _uofw.ProductRepository.GetProductsPaginationAsync(productParameters);
        if (products is null)
        {
            return NotFound("Products not found...");
        }

        var metadata = new
        {
            products.TotalItems,
            products.PageSize,
            products.CurrentPage,
            products.TotalPages,
            products.HasNext,
            products.HasPrevious,
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

        var productsDto = _mapper.Map<List<ProductDTO>>(products);
        return productsDto;
    }

    [HttpGet("{id:int}", Name = "GetProduct")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> GetAsync(int id)
    {
        var product = await _uofw.ProductRepository.GetByIdAsync(s => s.Id == id);
        if (product is null)
        {
            return NotFound("Product not found...");
        }
        var productDto = _mapper.Map<ProductDTO>(product);
        return productDto;
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(ProductDTO productDto)
    {
        if (productDto is null)
        {
            return BadRequest();
        }

        var product = _mapper.Map<ProductModel>(productDto);

        _uofw.ProductRepository.Add(product);
        await _uofw.CommitAsync();

        productDto = _mapper.Map<ProductDTO>(product);

        return new CreatedAtRouteResult("GetProduct", new { id = productDto.Id }, productDto);
    }

    [HttpPut("{id:int}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<ActionResult> PutAsync(int id, ProductDTO productDto)
    {
        if (id != productDto.Id)
        {
            return BadRequest();
        }

        var product = _mapper.Map<ProductModel>(productDto);

        _uofw.ProductRepository.Update(product);
        await _uofw.CommitAsync();

        productDto = _mapper.Map<ProductDTO>(product);

        return Ok(productDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProductDTO>> DeleteAsync(int id)
    {
        var product = await _uofw.ProductRepository.GetByIdAsync(s => s.Id == id);
        if (product is null)
        {
            return NotFound("Product not found...");
        }
        _uofw.ProductRepository.Delete(product);
        await _uofw.CommitAsync();

        var productDto = _mapper.Map<ProductDTO>(product);

        return Ok(productDto);
    }
}
