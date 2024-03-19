using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class TableController : ControllerBase
{
    private readonly ITableService _tableService;

    private readonly IConfiguration _configuration;

    public TableController(ITableService tableService, IConfiguration configuration)
    {
        _tableService = tableService;
        _configuration = configuration;
    }

    [HttpGet]
    public IEnumerable<TableViewModel> Get()
    {
        return _tableService.GetAll().Select(t => new TableViewModel
        {
            Id = t.Id,
            Name = t.Name,
        });
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        Table? table = _tableService.GetById(id);
        if (table == null)
        {
            return NotFound();
        }

        TableViewModel tableViewModel = new()
        {
            Id = table.Id,
            Name = table.Name,
        };

        return Ok(tableViewModel);
    }

    [HttpPost]
    public IActionResult Post([FromBody] TableRequest tableRequest)
    {
        string id = Guid.NewGuid().ToString();
        string qrCode = _tableService.GenerateQrCode(_configuration["BackendUrl"], id);
        
        Table table = new()
        {
            Id = id,
            Name = tableRequest.Name,
            QrCode = qrCode,
        };

        _tableService.Create(table);

        return NoContent();
    }

    [HttpPut("{id}")]
    public IActionResult Put(string id, [FromBody] TableRequest tableRequest)
    {
        Table? table = _tableService.GetById(id);
        if (table== null)
        {
            return NotFound();
        }

        table.Name = tableRequest.Name;

        _tableService.Update(table);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        _tableService.Delete(id);

        return NoContent();
    }
}