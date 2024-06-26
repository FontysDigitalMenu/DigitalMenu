﻿using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Moq;

namespace UnitTests.Services;

[TestFixture]
public class IngredientServiceTests
{
    [SetUp]
    public void Setup()
    {
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _ingredientService =
            new IngredientService(_mockIngredientRepository.Object, new Mock<ITranslationService>().Object);
    }

    private Mock<IIngredientRepository> _mockIngredientRepository;

    private IngredientService _ingredientService;

    // [Test]
    // public async Task CreateIngredient_ValidIngredient_ReturnsCreatedIngredient()
    // {
    //     // Arrange
    //     Ingredient ingredient = new()
    //     {
    //         Name = "Test Ingredient",
    //         Stock = 10,
    //     };
    //
    //     _mockIngredientRepository.Setup(repo => repo.CreateIngredient(It.IsAny<Ingredient>()))
    //         .ReturnsAsync(ingredient);
    //
    //     // Act
    //     Ingredient? createdIngredient = await _ingredientService.CreateIngredient(ingredient, "en");
    //
    //     // Assert
    //     Assert.That(createdIngredient, Is.Not.Null);
    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(createdIngredient.Name, Is.EqualTo(ingredient.Name));
    //         Assert.That(createdIngredient.Stock, Is.EqualTo(ingredient.Stock));
    //     });
    // }

    [Test]
    public Task CreateIngredient_NullOrEmptyName_ThrowsArgumentException()
    {
        // Arrange
        Ingredient ingredient = new()
        {
            Name = "",
            Stock = 10,
        };

        // Act and Assert
        ArgumentException ex = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _ingredientService.CreateIngredient(ingredient, "en");
        });

        Assert.That(ex.Message, Is.EqualTo("Ingredient name cannot be null or empty. (Parameter 'Name')"));
        return Task.CompletedTask;
    }

    [Test]
    public Task CreateIngredient_NegativeStock_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Ingredient ingredient = new()
        {
            Name = "Test Ingredient",
            Stock = -10,
        };

        // Act and Assert
        ArgumentOutOfRangeException ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await _ingredientService.CreateIngredient(ingredient, "en");
        });
        Assert.Multiple(() =>
        {
            Assert.That(ex.ParamName, Is.EqualTo("Stock"));
            Assert.That(ex.Message, Is.EqualTo("Ingredient stock must be greater than 0. (Parameter 'Stock')"));
        });
        return Task.CompletedTask;
    }

    [Test]
    public async Task GetIngredientByNameAsync_ExistingIngredientName_ReturnsIngredient()
    {
        // Arrange
        const string ingredientName = "Existing Ingredient";
        Ingredient existingIngredient = new()
        {
            Name = ingredientName,
            Stock = 20,
        };

        _mockIngredientRepository.Setup(repo => repo.GetIngredientByNameAsync(ingredientName))
            .ReturnsAsync(existingIngredient);

        // Act
        Ingredient? result = await _ingredientService.GetIngredientByNameAsync(ingredientName);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(existingIngredient));
    }

    [Test]
    public async Task GetIngredientByNameAsync_NonExistingIngredientName_ReturnsNull()
    {
        // Arrange
        const string nonExistingIngredientName = "Non Existing Ingredient";

        _mockIngredientRepository.Setup(repo => repo.GetIngredientByNameAsync(nonExistingIngredientName))
            .ReturnsAsync((Ingredient)null);

        // Act
        Ingredient? result = await _ingredientService.GetIngredientByNameAsync(nonExistingIngredientName);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetIngredients_ReturnsListOfIngredients()
    {
        // Arrange
        List<Ingredient> mockIngredients =
        [
            new Ingredient { Name = "Ingredient 1", Stock = 10 },
            new Ingredient { Name = "Ingredient 2", Stock = 15 },
            new Ingredient { Name = "Ingredient 3", Stock = 20 },
        ];

        _mockIngredientRepository.Setup(repo => repo.GetIngredients())
            .ReturnsAsync(mockIngredients);

        // Act
        List<Ingredient> result = await _ingredientService.GetIngredients();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<List<Ingredient>>());
        CollectionAssert.AreEqual(mockIngredients, result);
    }

    [Test]
    public async Task DeleteIngredientsByMenuItemId_ValidMenuItemId_ReturnsTrue()
    {
        // Arrange
        const int validMenuItemId = 123;

        _mockIngredientRepository.Setup(repo => repo.DeleteIngredientsByMenuItemId(validMenuItemId))
            .ReturnsAsync(true);

        // Act
        bool result = await _ingredientService.DeleteIngredientsByMenuItemId(validMenuItemId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public Task DeleteIngredientsByMenuItemId_InvalidMenuItemId_ThrowsNotFoundException()
    {
        // Arrange
        const int invalidMenuItemId = -1;

        // Act and Assert
        NotFoundException ex = Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await _ingredientService.DeleteIngredientsByMenuItemId(invalidMenuItemId);
        });

        Assert.That(ex.Message, Is.EqualTo("Menu item id not found."));
        return Task.CompletedTask;
    }

    // [Test]
    // public async Task UpdateIngredient_ValidIngredient_ReturnsTrue()
    // {
    //     // Arrange
    //     Ingredient ingredient = new()
    //     {
    //         Id = 1,
    //         Name = "Updated Ingredient",
    //         Stock = 20,
    //     };
    //
    //     _mockIngredientRepository.Setup(repo => repo.UpdateIngredient(ingredient))
    //         .ReturnsAsync(true);
    //
    //     // Act
    //     bool result = await _ingredientService.UpdateIngredient(ingredient, "en");
    //
    //     // Assert
    //     Assert.That(result, Is.True);
    // }

    [Test]
    public Task UpdateIngredient_NullName_ThrowsArgumentException()
    {
        // Arrange
        Ingredient ingredient = new()
        {
            Id = 1,
            Name = "",
            Stock = 20,
        };

        // Act and Assert
        ArgumentException ex = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _ingredientService.UpdateIngredient(ingredient, "en");
        });

        Assert.That(ex.Message, Is.EqualTo("Ingredient name cannot be null or empty. (Parameter 'Name')"));
        return Task.CompletedTask;
    }

    [Test]
    public Task UpdateIngredient_NegativeStock_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Ingredient ingredient = new()
        {
            Id = 1,
            Name = "Test Ingredient",
            Stock = -10,
        };

        // Act and Assert
        ArgumentOutOfRangeException ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await _ingredientService.UpdateIngredient(ingredient, "en");
        });

        Assert.Multiple(() =>
        {
            Assert.That(ex.ParamName, Is.EqualTo("Stock"));
            Assert.That(ex.Message, Is.EqualTo("Ingredient stock must be greater than 0. (Parameter 'Stock')"));
        });
        return Task.CompletedTask;
    }

    [Test]
    public async Task GetIngredientById_ExistingIngredientId_ReturnsIngredient()
    {
        // Arrange
        const int existingIngredientId = 1;
        Ingredient existingIngredient = new()
        {
            Id = existingIngredientId,
            Name = "Existing Ingredient",
            Stock = 20,
        };

        _mockIngredientRepository.Setup(repo => repo.GetIngredientById(existingIngredientId))
            .ReturnsAsync(existingIngredient);

        // Act
        Ingredient? result = await _ingredientService.GetIngredientById(existingIngredientId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(existingIngredient));
    }

    [Test]
    public async Task GetIngredientById_NonExistingIngredientId_ReturnsNull()
    {
        // Arrange
        const int nonExistingIngredientId = 100;

        _mockIngredientRepository.Setup(repo => repo.GetIngredientById(nonExistingIngredientId))
            .ReturnsAsync((Ingredient)null);

        // Act
        Ingredient? result = await _ingredientService.GetIngredientById(nonExistingIngredientId);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteIngredient_ValidIngredientId_ReturnsTrue()
    {
        // Arrange
        const int validIngredientId = 1;

        _mockIngredientRepository.Setup(repo => repo.DeleteIngredient(validIngredientId))
            .ReturnsAsync(true);

        // Act
        bool result = await _ingredientService.DeleteIngredient(validIngredientId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public Task DeleteIngredient_InvalidIngredientId_ThrowsNotFoundException()
    {
        // Arrange
        const int invalidIngredientId = -1;

        // Act and Assert
        NotFoundException ex = Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await _ingredientService.DeleteIngredient(invalidIngredientId);
        });

        Assert.That(ex.Message, Is.EqualTo("Ingredient id not found."));
        return Task.CompletedTask;
    }
}