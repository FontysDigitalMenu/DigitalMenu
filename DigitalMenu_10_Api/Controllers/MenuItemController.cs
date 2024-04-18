using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.Services;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/menuItem")]
[ApiController]
public class MenuItemController(
    IWebHostEnvironment webHostEnvironment,
    IMenuItemService menuItemService,
    ICategoryService categoryService,
    IIngredientService ingredientService
) : ControllerBase
{
    private readonly ImageService _imageService = new(webHostEnvironment);

    [HttpGet]
    public IActionResult Get(int lastId, int amount)
    {
        List<MenuItem> menuItems = (List<MenuItem>)menuItemService.GetNextMenuItems(lastId, amount);
        List<MenuItemViewModel> menuItemViewModels = new();
        foreach (MenuItem menuItem in menuItems)
        {
            MenuItemViewModel menuItemViewModel = new()
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                ImageUrl = menuItem.ImageUrl,
            };
            menuItemViewModels.Add(menuItemViewModel);
        }

        Log.Information("Get menu items {@menuItems}", menuItemViewModels);

        return Ok(menuItemViewModels);
    }

    [HttpGet("getCategories")]
    public IActionResult GetCategories(int lastId, int amount)
    {
        List<Category> categories = (List<Category>)menuItemService.GetCategoriesWithNextMenuItems(lastId, amount);

        List<CategoryViewModel> categoryViewModels = categories.Select(category => new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            MenuItems = category.MenuItems.Select(menuItem => new MenuItemViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                ImageUrl = menuItem.ImageUrl,
            }).ToList(),
        }).ToList();

        return Ok(categoryViewModels);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetMenuItem(int id)
    {
        MenuItem? menuitem = menuItemService.GetMenuItemById(id);
        if (menuitem == null)
        {
            return NotFound();
        }

        return Ok(menuitem);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("getMenuItems")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult> GetMenuItems()
    {
        List<MenuItem> menuItems = await menuItemService.GetMenuItems();

        List<MenuItemViewModel> menuItemViewModels = menuItems.Select(menuItem => new MenuItemViewModel
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Price = menuItem.Price,
            ImageUrl = menuItem.ImageUrl,
        }).ToList();

        return Ok(menuItemViewModels);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> CreateMenuItem([FromForm] MenuItemCreateRequest menuItemCreateRequest)
    {
        try
        {
            List<Category> categories = [];
            foreach (string categoryName in menuItemCreateRequest.Categories)
            {
                Category category = await categoryService.GetCategoryByName(categoryName) ??
                                    throw new NotFoundException("Category not found");
                categories.Add(category);
            }

            List<Ingredient> ingredients = [];

            foreach (string ingredientName in menuItemCreateRequest.IngredientsName)
            {
                Ingredient ingredient = await ingredientService.GetIngredientByNameAsync(ingredientName) ??
                                        throw new NotFoundException("Ingredient not found");
                ingredients.Add(ingredient);
            }

            string menuItemUrl = await _imageService.SaveImageAsync(menuItemCreateRequest.Image);

            MenuItem menuItem = new()
            {
                Name = menuItemCreateRequest.Name,
                Description = menuItemCreateRequest.Description,
                Price = (int)(menuItemCreateRequest.Price * 100),
                ImageUrl = string.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase,
                    menuItemUrl),
                Categories = categories,
            };

            MenuItem? createdMenuItem = await menuItemService.CreateMenuItem(menuItem);
            if (createdMenuItem == null)
            {
                return BadRequest(new { Message = "Menu item could not be created" });
            }

            List<MenuItemIngredient> menuItemIngredients = ingredients
                .Select(ingredient => new MenuItemIngredient
                {
                    IngredientId = ingredient.Id,
                    MenuItemId = createdMenuItem.Id,
                })
                .ToList();

            List<MenuItemIngredient>? createdMenuItemIngredients =
                await menuItemService.AddIngredientsToMenuItem(menuItemIngredients);
            if (createdMenuItemIngredients == null)
            {
                return BadRequest(new { Message = "Ingredients could not be added to the menu item" });
            }

            return Created("", new MenuItemViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                ImageUrl = menuItem.ImageUrl,
            });
        }
        catch (NotFoundException e)
        {
            return NotFound(new { e.Message });
        }
        catch (DatabaseCreationException e)
        {
            return BadRequest(new { e.Message });
        }
        catch (DatabaseUpdateException e)
        {
            return BadRequest(new { e.Message });
        }
    }
}