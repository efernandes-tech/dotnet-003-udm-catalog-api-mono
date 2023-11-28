using Api.DTOs;
using Api.Filters;
using Api.Models;
using Api.Paginations;
using Api.Repositories;
using Api.Services;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Api.Controllers.V1;

[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[EnableCors("AllowAdminRequest")]
[ApiConventionType(typeof(DefaultApiConventions))]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _uofw;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CategoriesController> _logger;
    private readonly IMapper _mapper;

    public CategoriesController(IUnitOfWork uofw, IConfiguration configuration,
        ILogger<CategoriesController> logger, IMapper mapper)
    {
        _uofw = uofw;
        _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet("/author")]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [EnableCors("AllowAdminRequest")]
    public string GetAuthor()
    {
        var author = _configuration["Author"];
        var log = _configuration["Logging:LogLevel:Default"];

        return $"Author: {author} | Log: {log}";
    }

    [HttpGet("/welcome/{name}")]
    public ActionResult<string> GetWelcome([FromServices] IMyService myService, string name)
    {
        return myService.Welcome(name);
    }

    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesWithProductsAsync(string name, bool onlyActivated = true)
    {
        _logger.LogInformation("========== ========== GET /categories/products ========== ==========");

        var categories = await _uofw.CategoryRepository.GetCategoryWithProductsAsync();

        return _mapper.Map<List<CategoryDTO>>(categories);
    }

    [HttpGet("/first-category")]
    [HttpGet("first-category")]
    public async Task<ActionResult<CategoryDTO>> GetFirstAsync()
    {
        var category = await _uofw.CategoryRepository.Get().FirstOrDefaultAsync();
        if (category is null)
        {
            return NotFound("Category not found...");
        }
        var categoryDto = _mapper.Map<CategoryDTO>(category);
        return Ok(categoryDto);
    }

    [HttpGet]
    [EnableQuery]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get([FromQuery] CategoryParameters categoryParameters)
    {
        try
        {
            var categories = await _uofw.CategoryRepository.GetCategoriesPaginationAsync(categoryParameters);
            if (categories is null)
            {
                return NotFound("Categories not found...");
            }
            var metadata = new
            {
                categories.TotalItems,
                categories.PageSize,
                categories.CurrentPage,
                categories.TotalPages,
                categories.HasNext,
                categories.HasPrevious,
            };

            Response?.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
            var categoriesDto = _mapper.Map<List<CategoryDTO>>(categories);
            return categoriesDto;
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Internal server error...");
        }
    }

    /// <summary>
    /// Get a Category by its Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <returns>Category object</returns>
    [HttpGet("{id:int:min(1)}/{name:alpha:length(5)}", Name = "GetCategory")]
    [EnableQuery]
    public async Task<ActionResult<CategoryDTO>> GetAsync(int id, [BindRequired] string name, [FromQuery] string description)
    {
        var category = await _uofw.CategoryRepository.GetByIdAsync(x => x.Id == id);

        _logger.LogInformation($"========== ========== GET /categories/{id} : {id} ========== ==========");

        if (category is null)
        {
            return NotFound("Category not found...");
        }
        var categoryDto = _mapper.Map<CategoryDTO>(category);
        return Ok(categoryDto);
    }

    /// <summary>
    /// Adds a new category
    /// </summary>
    /// <remarks>
    /// Example request:
    ///
    ///     POST api/v1/categories
    ///     {
    ///         "id": 1,
    ///         "name": "Category",
    ///         "description": "Category description",
    ///     }
    /// </remarks>
    /// <param name="categoryDto">Category object</param>
    /// <returns>Category object included</returns>
    /// <remarks>Return the category object included</remarks>
    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] CategoryDTO categoryDto)
    {
        if (categoryDto is null)
        {
            return BadRequest();
        }

        var category = _mapper.Map<CategoryModel>(categoryDto);

        _uofw.CategoryRepository.Add(category);
        await _uofw.CommitAsync();

        categoryDto = _mapper.Map<CategoryDTO>(category);

        return new CreatedAtRouteResult("GetCategory", new { id = categoryDto.Id }, categoryDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> PutAsync(int id, CategoryDTO categoryDto)
    {
        if (id != categoryDto.Id)
        {
            return BadRequest();
        }

        var category = _mapper.Map<CategoryModel>(categoryDto);

        _uofw.CategoryRepository.Update(category);
        await _uofw.CommitAsync();

        categoryDto = _mapper.Map<CategoryDTO>(category);

        return Ok(categoryDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<CategoryDTO>> DeleteAsync(int id)
    {
        var category = await _uofw.CategoryRepository.GetByIdAsync(x => x.Id == id);
        if (category is null)
        {
            return NotFound("Category not found...");
        }
        _uofw.CategoryRepository.Delete(category);
        await _uofw.CommitAsync();

        var categoryDto = _mapper.Map<CategoryDTO>(category);

        return Ok(categoryDto);
    }
}
