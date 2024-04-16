using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Moq;

namespace UnitTests.Services;

public class CartItemServiceTests
{
    private readonly Mock<ICartItemRepository> _mockCartItemRepository = new();
    
    private CartItemService _cartItemService = null!;
    
    [SetUp]
    public void Setup()
    {
        _cartItemService = new CartItemService(_mockCartItemRepository.Object);
    }
    
    [Test]
    public void GetByDeviceId_WhenCalled_ReturnsListOfCartItems()
    {
        // Arrange
        const string deviceId = "testDeviceId";
        List<CartItem> expectedCartItems =
        [
            new CartItem
            {
                Id = 1,
                Note = "Without salt",
                Quantity = 2,
                DeviceId = "testDeviceId",
                MenuItemId = 1,
            },
            new CartItem
            {
                Id = 2,
                Note = "Without salt",
                Quantity = 4,
                DeviceId = "testDeviceId",
                MenuItemId = 1,
            },
        ];
        _mockCartItemRepository.Setup(repo => repo.GetByDeviceId(deviceId)).Returns(expectedCartItems);
        
        // Act
        List<CartItem> result = _cartItemService.GetByDeviceId(deviceId);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedCartItems));
    }
    
    [Test]
    public void GetByMenuItemIdAndDeviceId_WhenCalled_ReturnsCartItemOrNull()
    {
        // Arrange
        const int menuItemId = 1;
        const string deviceId = "testDeviceId";
        CartItem expectedCartItem = new() { Id = 1, MenuItemId = menuItemId, DeviceId = deviceId };
        _mockCartItemRepository.Setup(repo => repo.GetByMenuItemIdAndDeviceId(menuItemId, deviceId))
            .Returns(expectedCartItem);
        
        // Act
        CartItem? result = _cartItemService.GetByMenuItemIdAndDeviceId(menuItemId, deviceId);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedCartItem));
    }
    
    [Test]
    public void GetCartItemsByMenuItemIdAndDeviceId_WhenCalled_ReturnsListOfCartItems()
    {
        // Arrange
        const int menuItemId = 1;
        const string deviceId = "testDeviceId";
        List<CartItem?> expectedCartItems =
        [
            new CartItem
            {
                Id = 1,
                Note = "Without salt",
                Quantity = 2,
                DeviceId = "testDeviceId",
                MenuItemId = 1,
            },
        ];
        _mockCartItemRepository.Setup(repo => repo.GetCartItemsByMenuItemIdAndDeviceId(menuItemId, deviceId))
            .Returns(expectedCartItems);
        
        // Act
        List<CartItem?> result = _cartItemService.GetCartItemsByMenuItemIdAndDeviceId(menuItemId, deviceId);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedCartItems));
    }
    
    [Test]
    public void Create_WhenCalled_ReturnsTrue()
    {
        // Arrange
        CartItem cartItem = new()
        {
            Id = 1,
            Note = "Without salt",
            Quantity = 2,
            DeviceId = "testDeviceId",
            MenuItemId = 1,
        };
        
        _mockCartItemRepository.Setup(repo => repo.Create(cartItem)).Returns(true);
        
        // Act
        bool result = _cartItemService.Create(cartItem);
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void ExistsByDeviceId_WhenCalled_ReturnsTrue()
    {
        // Arrange
        const string deviceId = "testDeviceId";
        
        _mockCartItemRepository.Setup(repo => repo.ExistsByDeviceId(deviceId)).Returns(true);
        
        // Act
        bool result = _cartItemService.ExistsByDeviceId(deviceId);
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void Delete_WhenCalled_ReturnsTrue()
    {
        // Arrange
        CartItem cartItem = new();
        
        _mockCartItemRepository.Setup(repo => repo.Delete(cartItem)).Returns(true);
        
        // Act
        bool result = _cartItemService.Delete(cartItem);
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void Update_WhenCalled_ReturnsTrue()
    {
        // Arrange
        CartItem cartItem = new();
        
        _mockCartItemRepository.Setup(repo => repo.Update(cartItem)).Returns(true);
        
        // Act
        bool result = _cartItemService.Update(cartItem);
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void AddExcludedIngredientToCartItem_WhenCalled_ReturnsTrue()
    {
        // Arrange
        ExcludedIngredientCartItem excludedIngredientCartItem = new();
        
        _mockCartItemRepository.Setup(repo => repo.AddExcludedIngredientToCartItem(excludedIngredientCartItem))
            .Returns(true);
        
        // Act
        bool result = _cartItemService.AddExcludedIngredientToCartItem(excludedIngredientCartItem);
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void GetExcludedIngredientsByCartItemId_WhenCalled_ReturnsListOfIngredients()
    {
        // Arrange
        const int cartItemId = 1;
        List<Ingredient> expectedIngredients =
        [
            new Ingredient(),
            new Ingredient(),
        ];
        _mockCartItemRepository.Setup(repo => repo.GetExcludedIngredientsByCartItemId(cartItemId))
            .Returns(expectedIngredients);
        
        // Act
        List<Ingredient> result = _cartItemService.GetExcludedIngredientsByCartItemId(cartItemId);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedIngredients));
    }
    
    [Test]
    public void GetByCartItemIdAndDeviceId_WhenCalled_ReturnsCartItemOrNull()
    {
        // Arrange
        const int cartItemId = 1;
        const string deviceId = "testDeviceId";
        CartItem expectedCartItem = new() { Id = cartItemId, DeviceId = deviceId };
        _mockCartItemRepository.Setup(repo => repo.GetByCartItemIdAndDeviceId(cartItemId, deviceId))
            .Returns(expectedCartItem);
        
        // Act
        CartItem? result = _cartItemService.GetByCartItemIdAndDeviceId(cartItemId, deviceId);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedCartItem));
    }
    
    [Test]
    public void DeleteExcludedIngredientsFromCartItem_WhenCalled_ReturnsTrue()
    {
        // Arrange
        const int cartItemId = 1;
        
        _mockCartItemRepository.Setup(repo => repo.DeleteExcludedIngredientsFromCartItem(cartItemId)).Returns(true);
        
        // Act
        bool result = _cartItemService.DeleteExcludedIngredientsFromCartItem(cartItemId);
        
        // Assert
        Assert.That(result, Is.True);
    }
}