using System.Security.Claims;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{email}")]
    public IEnumerable<UserViewModel> Get(string email)
    {
        return _userService.SearchByEmail(email).Select(x => new UserViewModel
        {
            Id = x.Id, Name = x.UserName, Email = x.Email,
        });
    }

    [HttpGet("info")]
    [Authorize]
    public List<string>? GetInfo()
    {
        string? id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (id == null)
        {
            return null;
        }

        return User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(r => r.Value).ToList();
    }
}