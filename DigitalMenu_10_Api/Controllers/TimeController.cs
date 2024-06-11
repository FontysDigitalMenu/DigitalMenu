using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/time")]
[ApiController]
public class TimeController(ITimeService timeService) : ControllerBase
{
    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public IActionResult Update([FromForm] TimeRequest timeRequest)
    {
        if (timeRequest.Hours >= 0 && timeRequest.Minutes >= 0)
        {
            Time time = new()
            {
                Hours = timeRequest.Hours.Value,
                Minutes = timeRequest.Minutes.Value,
            };
            timeService.UpdateOrCreate(time);
        }
        else
        {
            timeService.Delete();
        }

        return Ok();
    }
}