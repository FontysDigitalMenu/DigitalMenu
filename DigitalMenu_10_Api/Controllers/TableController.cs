using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v1/table")]
[ApiController]
public class TableController(ITableService tableService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    public IActionResult Get()
    {
        return Ok(tableService.GetAll().Select(t => new TableViewModel { Id = t.Id, Name = t.Name, SessionId = t.SessionId}));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public IActionResult Get([FromRoute] string id)
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
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public IActionResult Post([FromBody] TableRequest tableRequest)
    {
        string id = Guid.NewGuid().ToString();
        string sessionId = Guid.NewGuid().ToString();

        Table table = new() { Id = id, Name = tableRequest.Name , SessionId = sessionId};

        Table? createdTable = tableService.Create(table);
        if (createdTable == null)
        {
            return BadRequest(new { Message = "Table could not be created" });
        }

        return CreatedAtAction("Get", new { id = createdTable.Id },
            new TableViewModel { Id = createdTable.Id, Name = createdTable.Name });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult Put(string id, [FromBody] TableRequest tableRequest)
    {
        Table? table = tableService.GetById(id);
        if (table == null)
        {
            return NotFound();
        }

        table.Name = tableRequest.Name;

        if (!tableService.Update(table))
        {
            return BadRequest(new { Message = "Table could not be updated" });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult Delete(string id)
    {
        if (tableService.GetById(id) == null)
        {
            return NotFound();
        }

        if (!tableService.Delete(id))
        {
            return BadRequest(new { Message = "Table could not be deleted" });
        }

        return NoContent();
    }
    
    [HttpPost("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult ResetSession(string id)
    {
        try
        {
            if (!tableService.ResetSession(id))
            {            
                return BadRequest(new { Message = "Session could not be reset" });
            }
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}