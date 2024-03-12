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

    public TableController(ITableService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet]
    public IEnumerable<TableViewModel> Get()
    {
        return _tableService.GetAll().Select(t => new TableViewModel
        {
            Id = t.Id,
            Name = t.Name,
            QrCode = t.QrCode
        });
    }

    [HttpGet("{id:int}")]
    public TableViewModel? Get(int id)
    {
        Table? table = _tableService.GetById(id);
        if (table == null)
        {
            return null;
        }

        TableViewModel tableViewModel = new()
        {
            Id = table.Id,
            Name = table.Name,
            QrCode = table.QrCode
        };

        return tableViewModel;
    }

    [HttpPost]
    public void Post([FromBody] TableRequest tableRequest)
    {
        Table table = new()
        {
            Name = tableRequest.Name
        };

        _tableService.Create(table);
    }

    [HttpPut("{id:int}")]
    public void Put(int id, [FromBody] TableRequest tableRequest)
    {
        Table table = new()
        {
            Id = id,
            Name = tableRequest.Name
        };

        _tableService.Update(table);
    }

    [HttpDelete("{id:int}")]
    public void Delete(int id)
    {
        _tableService.Delete(id);
    }
}