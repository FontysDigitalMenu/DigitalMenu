using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/user")]
[ApiController]
public class UserController : Controller
{
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