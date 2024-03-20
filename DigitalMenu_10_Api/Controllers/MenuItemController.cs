using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class MenuItemController : ControllerBase
{
    private readonly IMenuItemService _menuItemService;

    public MenuItemController(IMenuItemService menuItemService)
    {
        _menuItemService = menuItemService;
    }

    [HttpGet("Paged")]
    public IActionResult Get(int lastId, int amount)
    {
        List<MenuItem> menuItems = (List<MenuItem>)_menuItemService.GetNextMenuItems(lastId, amount);
        List<MenuItemViewModel> menuItemViewModels = new();
        foreach (MenuItem menuItem in menuItems)
        {
            MenuItemViewModel menuItemViewModel = new()
            {
                Id = menuItem.Id, Name = menuItem.Name, Price = menuItem.Price, ImageUrl = menuItem.ImageUrl,
            };
            menuItemViewModels.Add(menuItemViewModel);
        }

        return Ok(menuItemViewModels);
    }

    [HttpGet("Paged/GetCategories")]
    public IActionResult GetCategories(int lastId, int amount)
    {
        List<Category> categories = (List<Category>)_menuItemService.GetCategoriesWithNextMenuItems(lastId, amount);

        List<CategoryViewModel> categoryViewModels = categories.Select(category => new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            MenuItemViewModels = category.MenuItems.Select(menuItem => new MenuItemViewModel
            {
                Id = menuItem.Id, Name = menuItem.Name, Price = menuItem.Price, ImageUrl = menuItem.ImageUrl,
            }).ToList(),
        }).ToList();

        return Ok(categoryViewModels);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetMenuItem(int id)
    {
        MenuItem menuitem = _menuItemService.GetMenuItemBy(id);
        return Ok(menuitem);
    }
}