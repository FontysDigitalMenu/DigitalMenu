using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;

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
        Reservation reservation = new()
        {
            Email = request.Email,
            ReservationDateTime = request.ReservationDateTime,
            TableId = request.TableId,
        };

        try
        {
            reservationService.Create(reservation);
        }
        catch (ReservationException e)
        {
            return BadRequest(new { e.Message });
        }

        return Created("n/a", ReservationViewModel.FromReservation(reservation));
    }
}