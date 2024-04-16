using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Moq;

namespace UnitTests.Services;

public class MenuItemServiceTests
{
    private readonly Mock<IMenuItemRepository> _mockMenuItemRepository = new();

    private MenuItemService _menuItemService = null!;

    [SetUp]
    public void Setup()
    {
        _menuItemService = new MenuItemService(_mockMenuItemRepository.Object);
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
        int id = 2;
        _mockMenuItemRepository.Setup(repo => repo.Delete(id)).Returns(true);
        _mockMenuItemRepository.Setup(repo => repo.GetMenuItemBy(id)).Returns(menuItem);

        // Act
        bool result = _menuItemService.Delete(id);

        // Assert
        Assert.IsTrue(result);
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
        Assert.Throws<NotFoundException>(Delete);
    }
}