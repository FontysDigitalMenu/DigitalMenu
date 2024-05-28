using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/settings")]
[ApiController]
public class SettingController(ISettingService settingService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetSettings()
    {
        Setting? setting = await settingService.GetSettings();

        if (setting != null)
        {
            return Ok(setting);
        }

        return NotFound();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateSettings(SettingUpdateRequest settingUpdateRequest)
    {
        try
        {
            Setting setting = new()
            {
                Id = settingUpdateRequest.Id,
                CompanyName = settingUpdateRequest.CompanyName,
                PrimaryColor = settingUpdateRequest.PrimaryColor,
                SecondaryColor = settingUpdateRequest.SecondaryColor,
            };

            bool updatedSetting = await settingService.UpdateSettings(setting);

            if (updatedSetting)
            {
                return Ok("Settings updated successfully");
            }

            return BadRequest(new { Message = "Setting could not be updated" });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { e.Message });
        }
        catch (NotFoundException e)
        {
            return NotFound(new { e.Message });
        }
    }
}