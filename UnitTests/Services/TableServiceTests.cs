using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Moq;

namespace UnitTests.Services;

public class TableServiceTests
{
    private readonly Mock<ITableRepository> _tableRepositoryMock = new();

    private readonly TableService _tableService;

    public TableServiceTests()
    {
        _tableService = new TableService(_tableRepositoryMock.Object);
    }

    [Test]
    public void GetAll_ShouldReturnAllTables()
    {
        // Arrange
        List<Table> tables =
        [
            new Table { Id = "CA3D0ED8-78D6-4690-8952-89D7E1FC18A4", Name = "Table 1", CreatedAt = DateTime.Now },
            new Table { Id = "CA3D0ED8-78D6-4690-8952-89D7E1FC18A5", Name = "Table 2", CreatedAt = DateTime.Now },
        ];
        _tableRepositoryMock.Setup(x => x.GetAll())
            .Returns(tables);

        // Act
        List<Table> result = _tableService.GetAll();

        // Assert
        Assert.That(result, Is.EqualTo(tables));
    }
}