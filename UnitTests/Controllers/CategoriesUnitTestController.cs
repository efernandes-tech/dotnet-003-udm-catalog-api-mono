using Api.Context;
using Api.Controllers.V1;
using Api.DTOs;
using Api.DTOs.Mappings;
using Api.Paginations;
using Api.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnitTests.Controllers;

public class CategoriesUnitTestController
{
    private readonly IUnitOfWork _uofw;

    //private readonly IConfiguration _configuration;
    //private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public static DbContextOptions<CatalogoApiDbContext> dbContextOptions { get; }

    public static string connectionString = "Server=localhost;DataBase=CatalogoApiDB;Uid=catalogoApi;Pwd=!Q@W#e4r5t";

    static CategoriesUnitTestController()
    {
        dbContextOptions = new DbContextOptionsBuilder<CatalogoApiDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
    }

    public CategoriesUnitTestController()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });
        _mapper = config.CreateMapper();

        var context = new CatalogoApiDbContext(dbContextOptions);

        //DbUnitTestsMockInitializer db = new DbUnitTestsMockInitializer();
        //db.Seed(context);

        _uofw = new UnitOfWork(context);
    }

    [Fact]
    public void GetCategories_Return_OkResult()
    {
        // Arrange
        var controller = new CategoriesController(_uofw, null, null, _mapper);

        // Act
        var data = controller.Get(new CategoryParameters());

        // Assert
        Assert.IsType<List<CategoryDTO>>(data.Result.Value);
    }

    [Fact]
    public void GetCategories_Return_Status500InternalServerError()
    {
        // Arrange
        var controller = new CategoriesController(_uofw, null, null, _mapper);

        // Act
        var data = controller.Get(null);

        // Assert
        Assert.Null(data.Result.Value);
    }

    [Fact]
    public void GetCategories_MatchResult()
    {
        // Arrange
        var controller = new CategoriesController(_uofw, null, null, _mapper);
        // Act
        var data = controller.Get(new CategoryParameters());
        // Assert
        Assert.IsType<List<CategoryDTO>>(data.Result.Value);
        var categories = data.Result.Value.Should().BeAssignableTo<List<CategoryDTO>>().Subject;

        Assert.Equal("Cryptocurrencies", categories[0].Name);
    }

    [Fact]
    public async void GetCategoriesById_Return_OkResult()
    {
        //Arrange
        var controller = new CategoriesController(_uofw, null, null, _mapper);
        var catId = 2;

        //Act
        var data = await controller.GetAsync(catId, "", "");

        //Assert
        Assert.NotNull(data.Result);
    }

    [Fact]
    public async void GetCategoriesById_Return_NotFoundResult()
    {
        //Arrange
        var controller = new CategoriesController(_uofw, null, null, _mapper);
        var catId = 9999;

        //Act
        var data = await controller.GetAsync(catId, null, null);

        //Assert
        Assert.NotNull(data.Result);
    }

    [Fact]
    public void Post_Category_AddValidData_Return_CreatedResult()
    {
        //Arrange
        var controller = new CategoriesController(_uofw, null, null, _mapper);

        var cat = new CategoryDTO()
        {
            Name = "Unit Test Add",
            Description = ""
        };

        //Act
        var data = controller.PostAsync(cat);

        //Assert
        Assert.NotNull(data.Result);
    }

    [Fact]
    public void Put_Category_Update_ValidData_Return_OkResult()
    {
        //Arrange
        var controller = new CategoriesController(_uofw, null, null, _mapper);
        var catId = 3;

        //Act
        var catDto = new CategoryDTO();
        catDto.Id = catId;
        catDto.Name = "Unit Test Update";

        var updatedData = controller.PutAsync(catId, catDto);

        //Assert
        Assert.NotNull(updatedData.Result);
    }

    [Fact]
    public void Delete_Category_Return_OkResult()
    {
        //Arrange
        var controller = new CategoriesController(_uofw, null, null, _mapper);
        var catId = 4;

        //Act
        var data = controller.DeleteAsync(catId);

        //Assert
        Assert.NotNull(data.Result);
    }
}
