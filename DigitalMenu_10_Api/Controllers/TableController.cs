using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v1/table")]
[ApiController]
public class TableController(ITableService tableService, IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    [HttpGet]
    public IEnumerable<TableViewModel> Get()
    {
        return tableService.GetAll().Select(t => new TableViewModel { Id = t.Id, Name = t.Name });
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        Table? table = tableService.GetById(id);
        if (table == null)
        {
            return NotFound();
        }

        TableViewModel tableViewModel = new() { Id = table.Id, Name = table.Name };

        return Ok(tableViewModel);
    }

    [HttpPost]
    public IActionResult Post([FromBody] TableRequest tableRequest)
    {
        string id = Guid.NewGuid().ToString();
        string qrCode = "n/a"; // _tableService.GenerateQrCode(_configuration["BackendUrl"], id);

        Table table = new() { Id = id, Name = tableRequest.Name, QrCode = qrCode };

        tableService.Create(table);

        return NoContent();
    }

    [HttpPut("{id}")]
    public IActionResult Put(string id, [FromBody] TableRequest tableRequest)
    {
        Table? table = tableService.GetById(id);
        if (table == null)
        {
            return NotFound();
        }

        table.Name = tableRequest.Name;

        tableService.Update(table);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        tableService.Delete(id);

        return NoContent();
    }
}