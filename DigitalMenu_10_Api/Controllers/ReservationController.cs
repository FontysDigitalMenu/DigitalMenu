using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Dtos;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/reservation")]
[ApiController]
public class ReservationController(
    IReservationService reservationService
) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public ActionResult<Reservation> Post([FromBody] ReservationRequest request)
    {
        Request.Headers.TryGetValue("Accept-Language", out StringValues locale);
        string localeValue = locale.FirstOrDefault() ?? "en";
        if (localeValue.Length > 2) localeValue = "en";

        Reservation reservation = new()
        {
            Email = request.Email,
            ReservationDateTime = request.ReservationDateTime,
        };

        try
        {
            reservationService.Create(reservation, localeValue);
        }
        catch (ReservationException e)
        {
            return BadRequest(new { e.Message });
        }

        return Created("n/a", ReservationViewModel.FromReservation(reservation));
    }

    // GET api/v1/reservation/availableTimes/5-22-2024
    [HttpGet("availableTimes/{date:datetime}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public ActionResult<IEnumerable<AvailableTimesViewModel>> Get([FromRoute] DateTime date)
    {
        List<AvailableTimes> availableTimes = reservationService.GetAvailableTimes(date);

        return Ok(availableTimes.Select(at => new AvailableTimesViewModel
        {
            endDateTime = at.endDateTime,
            startDateTime = at.startDateTime,
        }));
    }

    [HttpDelete("{reservationId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(500)]
    public IActionResult Delete([FromRoute] string reservationId)
    {
        reservationService.Delete(reservationId);
        return NoContent();
    }


    [HttpGet("{date:datetime}")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public IActionResult GetReservations([FromRoute] DateTime date)
    {
        List<Reservation> reservations = reservationService.GetReservations(date);

        return Ok(reservations.Select(r => new ReservationViewModel
        {
            Id = r.Id,
            Email = r.Email,
            ReservationDateTime = r.ReservationDateTime,
            ReservationId = r.ReservationId,
            TableName = r.Table.Name,
        }));
    }

    [HttpGet("{reservationId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public IActionResult GetReservation([FromRoute] string reservationId)
    {
        try
        {
            Reservation? reservation = reservationService.GetReservationById(reservationId);

            return Ok(new ReservationViewModel
            {
                Id = reservation!.Id,
                Email = reservation.Email,
                ReservationDateTime = reservation.ReservationDateTime,
                ReservationId = reservation.ReservationId,
                TableName = reservation.Table.Name,
            });
        }
        catch (ReservationException e)
        {
            return BadRequest(new { e.Message });
        }
    }
}