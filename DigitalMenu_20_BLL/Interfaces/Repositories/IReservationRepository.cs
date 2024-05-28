using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IReservationRepository
{
    public Reservation? GetBy(string tableId, DateTime reservationReservationDateTime);

    public Reservation? Create(Reservation reservation);

    public List<Reservation> GetByDay(DateTime dateTime);

    public void Delete(int reservationId);

    public void Unlock(int id);

    public List<Reservation> GetReservations(DateTime dateTime);
}