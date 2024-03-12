using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers
{
    [Route("api/[controller]")]
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
            var menuItems = (List<MenuItem>)_menuItemService.GetNextMenuItems(lastId, amount);
            List<MenuItemViewModel> menuItemViewModels = new List<MenuItemViewModel>();
            foreach (var menuItem in menuItems)
            {
                MenuItemViewModel menuItemViewModel = new MenuItemViewModel
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
}
