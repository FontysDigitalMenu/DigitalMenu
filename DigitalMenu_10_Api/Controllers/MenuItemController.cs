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

    [HttpGet("{amount:int}")]
    public IActionResult Get(int lastId, int amount)
    {
        List<MenuItem> menuItems = (List<MenuItem>)_menuItemService.GetNextMenuItems(lastId, amount);
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
        return Ok(menuItemViewModels);
    }
}