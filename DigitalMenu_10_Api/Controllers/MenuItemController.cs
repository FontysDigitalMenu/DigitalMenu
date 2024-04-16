using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/menuItem")]
[ApiController]
public class MenuItemController(IMenuItemService menuItemService) : ControllerBase
{
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
            MenuItemViewModels = category.MenuItems.Select(menuItem => new MenuItemViewModel
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
}