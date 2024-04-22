using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Moq;

namespace UnitTests.Services;

[TestFixture]
public class CategoryServiceTests
{
    [SetUp]
    public void Setup()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _categoryService = new CategoryService(_mockCategoryRepository.Object);
    }

    private Mock<ICategoryRepository> _mockCategoryRepository;

    private CategoryService _categoryService;

    [Test]
    public async Task GetCategories_ReturnsCategories()
    {
        // Arrange
        List<Category> categories = [new Category { Name = "Category1" }, new Category { Name = "Category2" }];
        _mockCategoryRepository.Setup(repo => repo.GetCategories()).ReturnsAsync(categories);

        // Act
        List<Category> result = await _categoryService.GetCategories();

        // Assert
        Assert.That(result, Is.EqualTo(categories));
    }

    [Test]
    public async Task GetCategoryByName_ReturnsCategory()
    {
        // Arrange
        const string categoryName = "Category1";
        Category category = new() { Name = categoryName };
        _mockCategoryRepository.Setup(repo => repo.GetCategoryByName(categoryName)).ReturnsAsync(category);

        // Act
        Category? result = await _categoryService.GetCategoryByName(categoryName);

        // Assert
        Assert.That(result, Is.EqualTo(category));
    }

    [Test]
    public void GetCategoryByName_EmptyName_ThrowsNotFoundException()
    {
        // Arrange
        const string categoryName = "";

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _categoryService.GetCategoryByName(categoryName));
    }

    [Test]
    public async Task CreateCategory_CreatesCategory()
    {
        // Arrange
        const string categoryName = "NewCategory";
        Category createdCategory = new() { Name = categoryName };
        _mockCategoryRepository.Setup(repo => repo.CreateCategory(It.IsAny<Category>())).ReturnsAsync(createdCategory);

        // Act
        Category result = await _categoryService.CreateCategory(categoryName);

        // Assert
        Assert.That(result, Is.EqualTo(createdCategory));
    }

    [Test]
    public void CreateCategory_NullOrEmptyName_ThrowsArgumentException()
    {
        // Arrange
        const string categoryName = "";

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _categoryService.CreateCategory(categoryName));
    }
}