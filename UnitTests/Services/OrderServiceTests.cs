using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Moq;

namespace UnitTests.Services;

public class OrderServiceTests
{
    private readonly Mock<ICartItemRepository> _cartItemRepositoryMock = new();

    private readonly Mock<IIngredientRepository> _ingredientRepository = new();

    private readonly Mock<IMenuItemRepository> _menuItemRepository = new();

    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();

    private readonly Mock<ISplitRepository> _splitRepositoryMock = new();

    private readonly Mock<ITableRepository> _tableRepositoryMock = new();

    private OrderService _orderService = null!;

    [SetUp]
    public void Setup()
    {
        _orderService = new OrderService(_orderRepositoryMock.Object, _cartItemRepositoryMock.Object,
            _tableRepositoryMock.Object, _splitRepositoryMock.Object, _menuItemRepository.Object,
            _ingredientRepository.Object);
    }

    // [Test]
    // public void GetTotalAmount_ShouldReturnTotalAmount()
    // {
    //     // Arrange
    //     const string tableSessionId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
    //     _cartItemRepositoryMock.Setup(x => x.ExistsByTableSessionId(tableSessionId))
    //         .Returns(true);
    //     _tableRepositoryMock.Setup(x => x.GetById(tableSessionId))
    //         .Returns(new Table());
    //     _cartItemRepositoryMock.Setup(x => x.GetByTableSessionId(tableSessionId))
    //         .Returns([
    //             new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
    //             new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
    //         ]);
    //
    //     // Act
    //     int result = _orderService.GetTotalAmount(tableSessionId);
    //
    //     // Assert
    //     Assert.That(result, Is.EqualTo(1259));
    // }

    [Test]
    public void GetTotalAmount_ShouldThrowDeviceNotFoundException()
    {
        // Arrange
        const string tableSessionId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        _cartItemRepositoryMock.Setup(x => x.ExistsByTableSessionId(tableSessionId))
            .Returns(false);
        _tableRepositoryMock.Setup(x => x.GetById(tableSessionId))
            .Returns(new Table());
        _cartItemRepositoryMock.Setup(x => x.GetByTableSessionId(tableSessionId))
            .Returns([
                new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
                new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
            ]);

        // Act
        // Assert
        Assert.Throws<NotFoundException>(() => _orderService.GetTotalAmount(tableSessionId));
    }

    [Test]
    public void GetTotalAmount_ShouldThrowTableNotFoundException()
    {
        // Arrange
        const string tableSessionId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        _cartItemRepositoryMock.Setup(x => x.ExistsByTableSessionId(tableSessionId))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(tableSessionId))
            .Returns((Table)null!);
        _cartItemRepositoryMock.Setup(x => x.GetByTableSessionId(tableSessionId))
            .Returns([
                new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
                new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
            ]);

        // Act
        // Assert
        Assert.Throws<NotFoundException>(() => _orderService.GetTotalAmount(tableSessionId));
    }

    // [Test]
    // public void GetTotalAmount_ShouldThrowEmptyCartItemsListNotFoundException()
    // {
    //     // Arrange
    //     const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
    //     const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
    //     _cartItemRepositoryMock.Setup(x => x.ExistsByTableSessionId(deviceId))
    //         .Returns(true);
    //     _tableRepositoryMock.Setup(x => x.GetById(tableId))
    //         .Returns(new Table());
    //     _cartItemRepositoryMock.Setup(x => x.GetByTableSessionId(deviceId))
    //         .Returns([]);
    //
    //     // Act
    //     // Assert
    //     Assert.Throws<NotFoundException>(() => _orderService.GetTotalAmount(deviceId, tableId));
    // }
    //
    // [Test]
    // public void Create_ShouldReturnOrder()
    // {
    //     // Arrange
    //     const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
    //     const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
    //     const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     Order order = new()
    //     {
    //         Id = orderId,
    //         TableId = tableId,
    //         TotalAmount = 1259,
    //     };
    //     List<Split> splits = [new Split { Amount = 1259, Name = "Split 1" }];
    //     _cartItemRepositoryMock.Setup(x => x.ExistsByTableSessionId(deviceId))
    //         .Returns(true);
    //     _tableRepositoryMock.Setup(x => x.GetById(tableId))
    //         .Returns(new Table());
    //     _cartItemRepositoryMock.Setup(x => x.GetByTableSessionId(deviceId))
    //         .Returns([
    //             new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
    //             new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
    //         ]);
    //     _cartItemRepositoryMock.Setup(x => x.GetExcludedIngredientsByCartItemId(0))
    //         .Returns(new List<Ingredient>());
    //     _orderRepositoryMock.Setup(x => x.Create(It.IsAny<Order>()))
    //         .Returns(order);
    //     _cartItemRepositoryMock.Setup(x => x.ClearByTableSessionId(deviceId))
    //         .Returns(true);
    //     _splitRepositoryMock.Setup(x => x.CreateSplits(splits))
    //         .Returns(true);
    //
    //     // Act
    //     Order result = _orderService.Create(order.DeviceId, order.TableId, splits);
    //
    //     // Assert
    //     Assert.That(result, Is.EqualTo(order));
    // }
    //
    // [Test]
    // public void Create_ShouldThrowDeviceIdNotFoundException()
    // {
    //     // Arrange
    //     const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
    //     const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
    //     const string paymentId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     Order order = new()
    //     {
    //         Id = orderId,
    //         DeviceId = deviceId,
    //         TableId = tableId,
    //         TotalAmount = 1259,
    //     };
    //     List<Split> splits = [new Split { Amount = 1259, Name = "Split 1" }];
    //     _cartItemRepositoryMock.Setup(x => x.ExistsByTableSessionId(deviceId))
    //         .Returns(false);
    //     _tableRepositoryMock.Setup(x => x.GetById(tableId))
    //         .Returns(new Table());
    //     _cartItemRepositoryMock.Setup(x => x.GetByTableSessionId(deviceId))
    //         .Returns([
    //             new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
    //             new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
    //         ]);
    //     _orderRepositoryMock.Setup(x => x.Create(It.IsAny<Order>()))
    //         .Returns(order);
    //     _cartItemRepositoryMock.Setup(x => x.ClearByTableSessionId(deviceId))
    //         .Returns(true);
    //
    //     // Act
    //     // Assert
    //     Assert.Throws<NotFoundException>(() => _orderService.Create(order.DeviceId, order.TableId, splits));
    // }
    //
    // [Test]
    // public void Create_ShouldThrowTableIdNotFoundException()
    // {
    //     // Arrange
    //     const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
    //     const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
    //     const string paymentId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     Order order = new()
    //     {
    //         Id = orderId,
    //         DeviceId = deviceId,
    //         TableId = tableId,
    //         TotalAmount = 1259,
    //     };
    //     List<Split> splits = [new Split { Amount = 1259, Name = "Split 1" }];
    //     _cartItemRepositoryMock.Setup(x => x.ExistsByTableSessionId(deviceId))
    //         .Returns(true);
    //     _tableRepositoryMock.Setup(x => x.GetById(tableId))
    //         .Returns((Table)null!);
    //     _cartItemRepositoryMock.Setup(x => x.GetByTableSessionId(deviceId))
    //         .Returns([
    //             new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
    //             new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
    //         ]);
    //     _orderRepositoryMock.Setup(x => x.Create(It.IsAny<Order>()))
    //         .Returns(order);
    //     _cartItemRepositoryMock.Setup(x => x.ClearByTableSessionId(deviceId))
    //         .Returns(true);
    //
    //     // Act
    //     // Assert
    //     Assert.Throws<NotFoundException>(() => _orderService.Create(order.DeviceId, order.TableId, splits));
    // }
    //
    // [Test]
    // public void Create_ShouldThrowCartItemsNotFoundException()
    // {
    //     // Arrange
    //     const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
    //     const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
    //     const string paymentId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     Order order = new()
    //     {
    //         Id = orderId,
    //         DeviceId = deviceId,
    //         TableId = tableId,
    //         TotalAmount = 1259,
    //     };
    //     List<Split> splits = [new Split { Amount = 1259, Name = "Split 1" }];
    //     _cartItemRepositoryMock.Setup(x => x.ExistsByTableSessionId(deviceId))
    //         .Returns(true);
    //     _tableRepositoryMock.Setup(x => x.GetById(tableId))
    //         .Returns(new Table());
    //     _cartItemRepositoryMock.Setup(x => x.GetByTableSessionId(deviceId))
    //         .Returns([]);
    //     _orderRepositoryMock.Setup(x => x.Create(It.IsAny<Order>()))
    //         .Returns(order);
    //     _cartItemRepositoryMock.Setup(x => x.ClearByTableSessionId(deviceId))
    //         .Returns(true);
    //
    //     // Act
    //     // Assert
    //     Assert.Throws<NotFoundException>(() => _orderService.Create(order.DeviceId, order.TableId, splits));
    // }
    //
    // [Test]
    // public void Create_ShouldThrowDatabaseCreationException()
    // {
    //     // Act
    //     const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
    //     const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
    //     const string paymentId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     List<Split> splits = [new Split { Amount = 1259, Name = "Split 1" }];
    //     _cartItemRepositoryMock.Setup(x => x.ExistsByTableSessionId(deviceId))
    //         .Returns(true);
    //     _tableRepositoryMock.Setup(x => x.GetById(tableId))
    //         .Returns(new Table());
    //     _cartItemRepositoryMock.Setup(x => x.GetByTableSessionId(deviceId))
    //         .Returns([
    //             new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
    //             new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
    //         ]);
    //     _cartItemRepositoryMock.Setup(x => x.GetExcludedIngredientsByCartItemId(0))
    //         .Returns(new List<Ingredient>());
    //     _orderRepositoryMock.Setup(x => x.Create(It.IsAny<Order>()))
    //         .Returns((Order)null!);
    //     _cartItemRepositoryMock.Setup(x => x.ClearByTableSessionId(deviceId))
    //         .Returns(true);
    //
    //     // Arrange
    //     // Assert
    //     Assert.Throws<DatabaseCreationException>(() => _orderService.Create(deviceId, tableId, splits));
    // }
    //
    // [Test]
    // public void GetBy_ShouldReturnOrder()
    // {
    //     // Arrange
    //     const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
    //     const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
    //     Order order = new()
    //     {
    //         Id = orderId,
    //         DeviceId = deviceId,
    //         TableId = tableId,
    //         TotalAmount = 1259,
    //     };
    //     _orderRepositoryMock.Setup(x => x.GetBy(orderId, deviceId, tableId))
    //         .Returns(order);
    //     _orderRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
    //         .Returns(true);
    //     _tableRepositoryMock.Setup(x => x.GetById(tableId))
    //         .Returns(new Table());
    //
    //     // Act
    //     Order? result = _orderService.GetBy(orderId, deviceId, tableId);
    //
    //     // Assert
    //     Assert.That(result, Is.EqualTo(order));
    // }
    //
    // [Test]
    // public void GetBy_ShouldThrowDeviceIdNotFoundException()
    // {
    //     // Arrange
    //     const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
    //     const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
    //     Order order = new()
    //     {
    //         Id = orderId,
    //         DeviceId = deviceId,
    //         TableId = tableId,
    //         TotalAmount = 1259,
    //     };
    //     _orderRepositoryMock.Setup(x => x.GetBy(orderId, deviceId, tableId))
    //         .Returns(order);
    //     _orderRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
    //         .Returns(false);
    //     _tableRepositoryMock.Setup(x => x.GetById(tableId))
    //         .Returns(new Table());
    //
    //     // Act
    //     // Assert
    //     Assert.Throws<NotFoundException>(() => _orderService.GetBy(orderId, deviceId, tableId));
    // }
    //
    // [Test]
    // public void GetBy_ShouldThrowTableIdNotFoundException()
    // {
    //     // Arrange
    //     const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    //     const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
    //     const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
    //     Order order = new()
    //     {
    //         Id = orderId,
    //         DeviceId = deviceId,
    //         TableId = tableId,
    //         TotalAmount = 1259,
    //     };
    //     _orderRepositoryMock.Setup(x => x.GetBy(orderId, deviceId, tableId))
    //         .Returns(order);
    //     _orderRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
    //         .Returns(true);
    //     _tableRepositoryMock.Setup(x => x.GetById(tableId))
    //         .Returns((Table)null!);
    //
    //     // Act
    //     // Assert
    //     Assert.Throws<NotFoundException>(() => _orderService.GetBy(orderId, deviceId, tableId));
    // }
    //
    // [Test]
    // public void Update_ShouldReturnTrue()
    // {
    //     // Arrange
    //     Order order = new()
    //     {
    //         Id = "F91178CE-FFCE-40ED-955F-3471BC6A0586",
    //         DeviceId = "77C10784-D645-406F-869D-C653B19948F5",
    //         TableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672",
    //         TotalAmount = 1259,
    //     };
    //     _orderRepositoryMock.Setup(x => x.Update(order))
    //         .Returns(true);
    //
    //     // Act
    //     bool result = _orderService.Update(order);
    //
    //     // Assert
    //     Assert.That(result, Is.True);
    // }
    //
    // // [Test]
    // // public async Task CreateMolliePayment_ShouldReturnPaymentResponse()
    // // {
    // //     // Arrange
    // //     const int totalAmount = 1259;
    // //     const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
    // //     PaymentResponse expectedResponse = new();
    // //
    // //     _mollieHelperMock.Setup(client => client.CreatePayment(totalAmount, orderId))
    // //         .ReturnsAsync(expectedResponse);
    // //
    // //     // Act
    // //     PaymentResponse result = await _orderService.CreateMolliePayment(totalAmount, orderId);
    // //
    // //     // Assert
    // //     Assert.That(result, Is.Not.Null);
    // // }
    //
    // // [Test]
    // // public async Task GetMolliePayment_ShouldReturnPaymentResponse()
    // // {
    // //     // Arrange
    // //     const string externalPaymentId = "A54E9D75-50E0-4245-B0A9-A557B2DE5C07";
    // //     PaymentResponse expectedResponse = new();
    // //
    // //     _mollieHelperMock.Setup(client => client.GetPayment(externalPaymentId))
    // //         .ReturnsAsync(expectedResponse);
    // //
    // //     // Act
    // //     PaymentResponse result = await _orderService.GetPaymentFromMollie(externalPaymentId);
    // //
    // //     // Assert
    // //     Assert.That(result, Is.Not.Null);
    // // }
}