﻿using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Dtos;
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
    [Authorize(Roles = "Admin, Employee")]
    [HttpGet]
    [ProducesResponseType(200)]
    public IActionResult Get()
    {
        return Ok(tableService.GetAll().Select(t => new TableViewModel
            { Id = t.Id, Name = t.Name, SessionId = t.SessionId }));
    }

    [AllowAnonymous]
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

        TableViewModel tableViewModel = new()
            { Id = table.Id, Name = table.Name, SessionId = table.SessionId, IsReservable = table.IsReservable };

        return Ok(tableViewModel);
    }

    [AllowAnonymous]
    [HttpGet("sessionId/{tableSessionId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public IActionResult GetSessionId([FromRoute] string tableSessionId)
    {
        Table? table = tableService.GetBySessionId(tableSessionId);
        if (table == null)
        {
            return NotFound();
        }

        TableViewModel tableViewModel = new()
            { Id = table.Id, Name = table.Name, SessionId = table.SessionId, IsReservable = table.IsReservable };

        return Ok(tableViewModel);
    }

    [AllowAnonymous]
    [HttpGet("scan/{id}/{code:int?}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public IActionResult Scan([FromRoute] string id, [FromRoute] int? code = null)
    {
        TableScan tableScan = tableService.Scan(id, code);
        Table? table = tableScan.Table;
        if (table == null)
        {
            return NotFound();
        }

        if (tableScan.IsReserved && !tableScan.IsUnlocked)
        {
            return Unauthorized(new
                { Message = "Please authorize yourself with the 6-digit code in your reservation verification mail" });
        }

        TableViewModel tableViewModel = new()
            { Id = table.Id, Name = table.Name, SessionId = table.SessionId, IsReservable = table.IsReservable };

        return Ok(tableViewModel);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public IActionResult Post([FromBody] TableRequest tableRequest)
    {
        string id = Guid.NewGuid().ToString();
        string sessionId = Guid.NewGuid().ToString();

        Table table = new()
            { Id = id, Name = tableRequest.Name, IsReservable = tableRequest.IsReservable, SessionId = sessionId };

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
        table.IsReservable = tableRequest.IsReservable;

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

    [Authorize(Roles = "Admin, Employee")]
    [HttpPost("ResetSession/{id}")]
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

    [HttpPost("AddHost")]
    [AllowAnonymous]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult AddHost([FromForm] string id, [FromForm] string deviceId)
    {
        try
        {
            if (!tableService.AddHost(id, deviceId))
            {
                return BadRequest(new { Message = "Host could not be added" });
            }

            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}