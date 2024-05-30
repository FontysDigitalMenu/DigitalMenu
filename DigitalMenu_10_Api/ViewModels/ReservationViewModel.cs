using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_10_Api.ViewModels;

public class ReservationViewModel
{
    public string Id { get; set; }

    public int ReservationId { get; set; }

    public string Email { get; set; }

    public DateTime ReservationDateTime { get; set; }

    public string TableName { get; set; }

    public static ReservationViewModel FromReservation(Reservation reservation)
    {
        return new ReservationViewModel
        {
            Id = reservation.Id,
            ReservationId = reservation.ReservationId,
            Email = reservation.Email,
            ReservationDateTime = reservation.ReservationDateTime,
            TableName = reservation.Table.Name,
        };
    }
}