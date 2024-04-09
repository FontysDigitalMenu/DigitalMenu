using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Helpers;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Mollie.Api.Models.Payment.Response;
using Moq;

namespace UnitTests.Services;

public class OrderServiceTests
{
    private readonly Mock<ICartItemRepository> _cartItemRepositoryMock = new();

    private readonly Mock<IMollieHelper> _mollieHelperMock = new();

    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();

    private readonly Mock<ITableRepository> _tableRepositoryMock = new();

    private OrderService _orderService = null!;

    [SetUp]
    public void Setup()
    {
        _orderService = new OrderService(_orderRepositoryMock.Object, _cartItemRepositoryMock.Object,
            _tableRepositoryMock.Object, _mollieHelperMock.Object);
    }

    [Test]
    public void GetTotalAmount_ShouldReturnTotalAmount()
    {
        // Arrange
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        _cartItemRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns(new Table());
        _cartItemRepositoryMock.Setup(x => x.GetByDeviceId(deviceId))
            .Returns([
                new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
                new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
            ]);

        // Act
        int result = _orderService.GetTotalAmount(deviceId, tableId);

        // Assert
        Assert.That(result, Is.EqualTo(1259));
    }

    [Test]
    public void GetTotalAmount_ShouldThrowDeviceNotFoundException()
    {
        // Arrange
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        _cartItemRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(false);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns(new Table());
        _cartItemRepositoryMock.Setup(x => x.GetByDeviceId(deviceId))
            .Returns([
                new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
                new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
            ]);

        // Act
        // Assert
        Assert.Throws<NotFoundException>(() => _orderService.GetTotalAmount(deviceId, tableId));
    }

    [Test]
    public void GetTotalAmount_ShouldThrowTableNotFoundException()
    {
        // Arrange
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        _cartItemRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns((Table)null!);
        _cartItemRepositoryMock.Setup(x => x.GetByDeviceId(deviceId))
            .Returns([
                new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
                new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
            ]);

        // Act
        // Assert
        Assert.Throws<NotFoundException>(() => _orderService.GetTotalAmount(deviceId, tableId));
    }

    [Test]
    public void GetTotalAmount_ShouldThrowEmptyCartItemsListNotFoundException()
    {
        // Arrange
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        _cartItemRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns(new Table());
        _cartItemRepositoryMock.Setup(x => x.GetByDeviceId(deviceId))
            .Returns([]);

        // Act
        // Assert
        Assert.Throws<NotFoundException>(() => _orderService.GetTotalAmount(deviceId, tableId));
    }

    [Test]
    public void Create_ShouldReturnOrder()
    {
        // Act
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        const string paymentId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        Order order = new()
        {
            Id = orderId,
            DeviceId = deviceId,
            TableId = tableId,
            TotalAmount = 1259,
        };
        _cartItemRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns(new Table());
        _cartItemRepositoryMock.Setup(x => x.GetByDeviceId(deviceId))
            .Returns([
                new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
                new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
            ]);
        _cartItemRepositoryMock.Setup(x => x.GetExcludedIngredientsByCartItemId(0))
            .Returns(new List<Ingredient>());
        _orderRepositoryMock.Setup(x => x.Create(It.IsAny<Order>()))
            .Returns(order);
        _cartItemRepositoryMock.Setup(x => x.ClearByDeviceId(deviceId))
            .Returns(true);

        // Arrange
        Order result = _orderService.Create(deviceId, tableId, paymentId, orderId);

        // Assert
        Assert.That(result, Is.EqualTo(order));
    }

    [Test]
    public void Create_ShouldThrowDeviceIdNotFoundException()
    {
        // Act
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        const string paymentId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        Order order = new()
        {
            Id = orderId,
            DeviceId = deviceId,
            TableId = tableId,
            TotalAmount = 1259,
        };
        _cartItemRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(false);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns(new Table());
        _cartItemRepositoryMock.Setup(x => x.GetByDeviceId(deviceId))
            .Returns([
                new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
                new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
            ]);
        _orderRepositoryMock.Setup(x => x.Create(It.IsAny<Order>()))
            .Returns(order);
        _cartItemRepositoryMock.Setup(x => x.ClearByDeviceId(deviceId))
            .Returns(true);

        // Arrange
        // Assert
        Assert.Throws<NotFoundException>(() => _orderService.Create(deviceId, tableId, paymentId, orderId));
    }

    [Test]
    public void Create_ShouldThrowTableIdNotFoundException()
    {
        // Act
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        const string paymentId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        Order order = new()
        {
            Id = orderId,
            DeviceId = deviceId,
            TableId = tableId,
            TotalAmount = 1259,
        };
        _cartItemRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns((Table)null!);
        _cartItemRepositoryMock.Setup(x => x.GetByDeviceId(deviceId))
            .Returns([
                new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
                new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
            ]);
        _orderRepositoryMock.Setup(x => x.Create(It.IsAny<Order>()))
            .Returns(order);
        _cartItemRepositoryMock.Setup(x => x.ClearByDeviceId(deviceId))
            .Returns(true);

        // Arrange
        // Assert
        Assert.Throws<NotFoundException>(() => _orderService.Create(deviceId, tableId, paymentId, orderId));
    }

    [Test]
    public void Create_ShouldThrowCartItemsNotFoundException()
    {
        // Act
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        const string paymentId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        Order order = new()
        {
            Id = orderId,
            DeviceId = deviceId,
            TableId = tableId,
            TotalAmount = 1259,
        };
        _cartItemRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns(new Table());
        _cartItemRepositoryMock.Setup(x => x.GetByDeviceId(deviceId))
            .Returns([]);
        _orderRepositoryMock.Setup(x => x.Create(It.IsAny<Order>()))
            .Returns(order);
        _cartItemRepositoryMock.Setup(x => x.ClearByDeviceId(deviceId))
            .Returns(true);

        // Arrange
        // Assert
        Assert.Throws<NotFoundException>(() => _orderService.Create(deviceId, tableId, paymentId, orderId));
    }

    [Test]
    public void Create_ShouldThrowDatabaseCreationException()
    {
        // Act
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        const string paymentId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        _cartItemRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns(new Table());
        _cartItemRepositoryMock.Setup(x => x.GetByDeviceId(deviceId))
            .Returns([
                new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
                new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
            ]);
        _cartItemRepositoryMock.Setup(x => x.GetExcludedIngredientsByCartItemId(0))
            .Returns(new List<Ingredient>());
        _orderRepositoryMock.Setup(x => x.Create(It.IsAny<Order>()))
            .Returns((Order)null!);
        _cartItemRepositoryMock.Setup(x => x.ClearByDeviceId(deviceId))
            .Returns(true);

        // Arrange
        // Assert
        Assert.Throws<DatabaseCreationException>(() => _orderService.Create(deviceId, tableId, paymentId, orderId));
    }

    [Test]
    public void Create_ShouldThrowDatabaseUpdateException()
    {
        // Act
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        const string paymentId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        Order order = new()
        {
            Id = orderId,
            DeviceId = deviceId,
            TableId = tableId,
            TotalAmount = 1259,
        };
        _cartItemRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns(new Table());
        _cartItemRepositoryMock.Setup(x => x.GetByDeviceId(deviceId))
            .Returns([
                new CartItem { MenuItem = new MenuItem { Price = 500 }, Quantity = 2 },
                new CartItem { MenuItem = new MenuItem { Price = 259 }, Quantity = 1 },
            ]);
        _orderRepositoryMock.Setup(x => x.Create(It.IsAny<Order>()))
            .Returns(order);
        _cartItemRepositoryMock.Setup(x => x.ClearByDeviceId(deviceId))
            .Returns(false);

        // Arrange
        // Assert
        Assert.Throws<DatabaseUpdateException>(() => _orderService.Create(deviceId, tableId, paymentId, orderId));
    }

    [Test]
    public void GetBy_ShouldReturnOrder()
    {
        // Arrange
        const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        Order order = new()
        {
            Id = orderId,
            DeviceId = deviceId,
            TableId = tableId,
            TotalAmount = 1259,
        };
        _orderRepositoryMock.Setup(x => x.GetBy(orderId, deviceId, tableId))
            .Returns(order);
        _orderRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns(new Table());

        // Act
        Order? result = _orderService.GetBy(orderId, deviceId, tableId);

        // Assert
        Assert.That(result, Is.EqualTo(order));
    }

    [Test]
    public void GetBy_ShouldThrowDeviceIdNotFoundException()
    {
        // Arrange
        const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        Order order = new()
        {
            Id = orderId,
            DeviceId = deviceId,
            TableId = tableId,
            TotalAmount = 1259,
        };
        _orderRepositoryMock.Setup(x => x.GetBy(orderId, deviceId, tableId))
            .Returns(order);
        _orderRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(false);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns(new Table());

        // Act
        // Assert
        Assert.Throws<NotFoundException>(() => _orderService.GetBy(orderId, deviceId, tableId));
    }

    [Test]
    public void GetBy_ShouldThrowTableIdNotFoundException()
    {
        // Arrange
        const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        const string deviceId = "77C10784-D645-406F-869D-C653B19948F5";
        const string tableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672";
        Order order = new()
        {
            Id = orderId,
            DeviceId = deviceId,
            TableId = tableId,
            TotalAmount = 1259,
        };
        _orderRepositoryMock.Setup(x => x.GetBy(orderId, deviceId, tableId))
            .Returns(order);
        _orderRepositoryMock.Setup(x => x.ExistsByDeviceId(deviceId))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(tableId))
            .Returns((Table)null!);

        // Act
        // Assert
        Assert.Throws<NotFoundException>(() => _orderService.GetBy(orderId, deviceId, tableId));
    }

    [Test]
    public void Update_ShouldReturnTrue()
    {
        // Arrange
        Order order = new()
        {
            Id = "F91178CE-FFCE-40ED-955F-3471BC6A0586",
            DeviceId = "77C10784-D645-406F-869D-C653B19948F5",
            TableId = "11CCAB02-0C97-41F7-8F35-18CFD4BA4672",
            TotalAmount = 1259,
        };
        _orderRepositoryMock.Setup(x => x.Update(order))
            .Returns(true);

        // Act
        bool result = _orderService.Update(order);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CreateMolliePayment_ShouldReturnPaymentResponse()
    {
        // Arrange
        const int totalAmount = 1259;
        const string orderId = "F91178CE-FFCE-40ED-955F-3471BC6A0586";
        PaymentResponse expectedResponse = new();

        _mollieHelperMock.Setup(client => client.CreatePayment(totalAmount, orderId))
            .ReturnsAsync(expectedResponse);

        // Act
        PaymentResponse result = await _orderService.CreateMolliePayment(totalAmount, orderId);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetMolliePayment_ShouldReturnPaymentResponse()
    {
        // Arrange
        const string externalPaymentId = "A54E9D75-50E0-4245-B0A9-A557B2DE5C07";
        PaymentResponse expectedResponse = new();

        _mollieHelperMock.Setup(client => client.GetPayment(externalPaymentId))
            .ReturnsAsync(expectedResponse);

        // Act
        PaymentResponse result = await _orderService.GetPaymentFromMollie(externalPaymentId);

        // Assert
        Assert.That(result, Is.Not.Null);
    }
}