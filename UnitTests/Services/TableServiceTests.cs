﻿using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Moq;

namespace UnitTests.Services;

public class TableServiceTests
{
    private readonly Mock<IReservationRepository> _reservationRepositoryMock = new();

    private readonly Mock<ITableRepository> _tableRepositoryMock = new();

    private readonly Mock<ITimeService> _timeServiceMock = new();

    private TableService _tableService = null!;

    [SetUp]
    public void Setup()
    {
        ReservationService reservationService =
            new(_reservationRepositoryMock.Object, _tableRepositoryMock.Object, null!, _timeServiceMock.Object);
        _tableService = new TableService(_tableRepositoryMock.Object, reservationService, _timeServiceMock.Object);
    }

    // [Test]
    // public void GetAll_ShouldReturnAllTables()
    // {
    //     // Arrange
    //     List<Table> tables =
    //     [
    //         new Table
    //         {
    //             Id = "CA3D0ED8-78D6-4690-8952-89D7E1FC18A4", Name = "Table 1", CreatedAt = timeser.GetNow(),
    //         },
    //         new Table { Id = "B9D09C27-D862-4DBF-A250-A966846DE1E0", Name = "Table 2", CreatedAt = DateTime.Now },
    //     ];
    //     _tableRepositoryMock.Setup(x => x.GetAll())
    //         .Returns(tables);
    //
    //     // Act
    //     List<Table> result = _tableService.GetAll();
    //
    //     // Assert
    //     Assert.That(result, Is.EqualTo(tables));
    // }

    [Test]
    public void Create_ShouldReturnCreatedTable()
    {
        // Arrange
        Table table = new() { Id = "CA3D0ED8-78D6-4690-8952-89D7E1FC18A4", Name = "Table 1", CreatedAt = DateTime.Now };
        _tableRepositoryMock.Setup(x => x.Create(table))
            .Returns(table);

        // Act
        Table? result = _tableService.Create(table);

        // Assert
        Assert.That(result, Is.EqualTo(table));
    }

    [Test]
    public void GetById_ShouldReturnTableById()
    {
        // Arrange
        Table table = new() { Id = "CA3D0ED8-78D6-4690-8952-89D7E1FC18A4", Name = "Table 1", CreatedAt = DateTime.Now };
        _tableRepositoryMock.Setup(x => x.GetById(table.Id))
            .Returns(table);

        // Act
        Table? result = _tableService.GetById(table.Id);

        // Assert
        Assert.That(result, Is.EqualTo(table));
    }

    [Test]
    public void Update_ShouldReturnTrue()
    {
        // Arrange
        Table table = new() { Id = "CA3D0ED8-78D6-4690-8952-89D7E1FC18A4", Name = "Table 1", CreatedAt = DateTime.Now };
        _tableRepositoryMock.Setup(x => x.Update(table))
            .Returns(true);

        // Act
        bool result = _tableService.Update(table);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Delete_ShouldReturnTrue()
    {
        // Arrange
        const string id = "CA3D0ED8-78D6-4690-8952-89D7E1FC18A4";
        _tableRepositoryMock.Setup(x => x.Delete(id))
            .Returns(true);

        // Act
        bool result = _tableService.Delete(id);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void ResetSession_ShouldReturnTrue()
    {
        // Arrange
        Table table = new() { Id = "CA3D0ED8-78D6-4690-8952-89D7E1FC18A4", Name = "Table 1", CreatedAt = DateTime.Now };
        _tableRepositoryMock.Setup(x => x.Update(table))
            .Returns(true);
        _tableRepositoryMock.Setup(x => x.GetById(table.Id))
            .Returns(table);

        // Act
        bool result = _tableService.ResetSession("CA3D0ED8-78D6-4690-8952-89D7E1FC18A4");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void ResetSession_ThrowsNotFoundException()
    {
        // Arrange
        string id = "CA3D0ED8-78D6-4690-8952-89D7E1FC18A4";
        _tableRepositoryMock.Setup(x => x.GetById(id))
            .Returns((Table?)null);

        // Act
        void ResetSession()
        {
            _tableService.ResetSession(id);
        }

        // Assert
        Assert.Throws<NotFoundException>(ResetSession);
    }
}