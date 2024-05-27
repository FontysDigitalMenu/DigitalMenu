using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Moq;
using static NUnit.Framework.Assert;

namespace UnitTests.Services;

public class MenuItemServiceTests
{
    private MenuItemService _menuItemService;

    private Mock<IMenuItemRepository> _mockMenuItemRepository;

    [SetUp]
    public void Setup()
    {
        _mockMenuItemRepository = new Mock<IMenuItemRepository>();
        _menuItemService = new MenuItemService(_mockMenuItemRepository.Object);
    }

    [Test]
    public void GetCategoriesWithNextMenuItems_ReturnsCategoriesWithMenuItems()
    {
        // Arrange
        const int lastId = 0;
        const int amount = 2;
        List<MenuItem> menuItems =
        [
            new MenuItem
            {
                Id = 1, Name = "Pizza", Categories = [new Category { Id = 1, Name = "Italian" }],
            },
            new MenuItem
            {
                Id = 2, Name = "Burger", Categories = [new Category { Id = 2, Name = "American" }],
            },
        ];
        List<Category> categories =
        [
            new Category { Id = 1, Name = "Italian" },
            new Category { Id = 2, Name = "American" },
        ];
        _mockMenuItemRepository.Setup(repo => repo.GetNextMenuItemsWithCategory(lastId, amount)).Returns(menuItems);
        _mockMenuItemRepository.Setup(repo => repo.GetCategories()).Returns(categories);

        // Act
        IEnumerable<Category> result = _menuItemService.GetCategoriesWithNextMenuItems(lastId, amount);

        // Assert
        IEnumerable<Category> enumerable = result.ToList();
        That(enumerable, Is.Not.Null);
        That(enumerable.Count(), Is.EqualTo(2));
    }

    [Test]
    public void GetNextMenuItems_ReturnsNextMenuItems()
    {
        // Arrange
        const int lastId = 0;
        const int amount = 2;
        List<MenuItem> menuItems =
        [
            new MenuItem { Id = 1, Name = "Pizza" },
            new MenuItem { Id = 2, Name = "Burger" },
        ];
        _mockMenuItemRepository.Setup(repo => repo.GetNextMenuItems(lastId, amount)).Returns(menuItems);

        // Act
        IEnumerable<MenuItem> result = _menuItemService.GetNextMenuItems(lastId, amount);

        // Assert
        IEnumerable<MenuItem> enumerable = result.ToList();
        That(enumerable, Is.Not.Null);
        That(enumerable.Count(), Is.EqualTo(2));
    }

    [Test]
    public void GetMenuItemById_ReturnsMenuItem_WhenMenuItemExists()
    {
        // Arrange
        const int id = 123;
        MenuItem menuItem = new() { Id = id, Name = "Pizza" };
        _mockMenuItemRepository.Setup(repo => repo.GetMenuItemBy(id)).Returns(menuItem);

        // Act and Assert
        MenuItem? result = _menuItemService.GetMenuItemById(id);
        That(result, Is.Not.Null);
        That(result, Is.EqualTo(menuItem));
    }

    [Test]
    public void GetMenuItemById_ReturnsNull_WhenMenuItemDoesNotExist()
    {
        // Arrange
        const int id = 123;
        _mockMenuItemRepository.Setup(repo => repo.GetMenuItemBy(id)).Returns((MenuItem)null);

        // Act
        MenuItem? result = _menuItemService.GetMenuItemById(id);
        That(result, Is.Null);
    }

    [Test]
    public async Task GetMenuItems_ReturnsMenuItems()
    {
        // Arrange
        List<MenuItem> menuItems =
        [
            new MenuItem { Id = 1, Name = "Pizza" },
            new MenuItem { Id = 2, Name = "Burger" },
        ];
        _mockMenuItemRepository.Setup(repo => repo.GetMenuItems(0, 2)).ReturnsAsync(menuItems);

        // Act
        List<MenuItem> result = await _menuItemService.GetMenuItems(1, 2);
        That(result, Is.Not.Null);
        That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task CreateMenuItem_ReturnsCreatedMenuItem()
    {
        // Arrange
        MenuItem menuItem = new() { Name = "Pizza" };
        _mockMenuItemRepository.Setup(repo => repo.CreateMenuItem(menuItem)).ReturnsAsync(menuItem);

        // Act and Assert
        MenuItem? result = await _menuItemService.CreateMenuItem(menuItem);
        That(result, Is.Not.Null);
        That(result, Is.EqualTo(menuItem));
    }

    [Test]
    public async Task AddIngredientsToMenuItem_ReturnsAddedIngredients()
    {
        // Arrange
        List<MenuItemIngredient> menuItemIngredients = [];
        _mockMenuItemRepository.Setup(repo => repo.AddIngredientsToMenuItem(menuItemIngredients))
            .ReturnsAsync(menuItemIngredients);

        // Act and Assert
        List<MenuItemIngredient>? result = await _menuItemService.AddIngredientsToMenuItem(menuItemIngredients);
        That(result, Is.Not.Null);
        That(result, Is.EqualTo(menuItemIngredients));
    }

    [Test]
    public void Delete_ReturnsTrue()
    {
        // Arrange
        MenuItem menuItem = new()
        {
            Id = 2,
            Name = "Pizza",
            Description = "A delicious pizza",
            Price = 1500,
            ImageUrl =
                "https://www.moulinex-me.com/medias/?context=bWFzdGVyfHJvb3R8MTQzNTExfGltYWdlL2pwZWd8aGNlL2hmZC8xNTk2ODYyNTc4NjkxMC5qcGd8MmYwYzQ4YTg0MTgzNmVjYTZkMWZkZWZmMDdlMWFlMjRhOGIxMTQ2MTZkNDk4ZDU3ZjlkNDk2MzMzNDA5OWY3OA",
        };
        const int id = 2;
        _mockMenuItemRepository.Setup(repo => repo.Delete(id)).Returns(true);
        _mockMenuItemRepository.Setup(repo => repo.GetMenuItemBy(id)).Returns(menuItem);

        // Act
        bool result = _menuItemService.Delete(id);
        That(result, Is.True);
    }

    [Test]
    public void Delete_ThrowsNotFoundException()
    {
        // Arrange
        int id = 2234234;
        _mockMenuItemRepository.Setup(repo => repo.GetMenuItemBy(id)).Returns(null as MenuItem);

        // Act
        void Delete()
        {
            _menuItemService.Delete(id);
        }

        // Assert
        Throws<NotFoundException>(Delete);
    }
}