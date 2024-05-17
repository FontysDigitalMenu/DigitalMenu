using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IReservationRepository
{
    public Reservation? GetBy(string tableId, DateTime reservationReservationDateTime);

    public Reservation? Create(Reservation reservation);
}