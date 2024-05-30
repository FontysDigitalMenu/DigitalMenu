using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IReservationRepository
{
    public Reservation? GetBy(string tableId, DateTime reservationReservationDateTime);

    public Reservation? Create(Reservation reservation);

    public List<Reservation> GetByDay(DateTime dateTime);

    public void Delete(string reservationId);

    public void Unlock(string id);

    public List<Reservation> GetReservations(DateTime dateTime);

    public Reservation? GetReservationById(string reservationId);
}