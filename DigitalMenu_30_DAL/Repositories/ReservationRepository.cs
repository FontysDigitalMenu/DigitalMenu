using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;

namespace DigitalMenu_30_DAL.Repositories;

public class ReservationRepository(ApplicationDbContext dbContext) : IReservationRepository
{
    public Reservation? GetBy(string tableId, DateTime reservationReservationDateTime)
    {
        return dbContext.Reservations.FirstOrDefault(r =>
            r.TableId == tableId && r.ReservationDateTime == reservationReservationDateTime);
    }

    public Reservation? Create(Reservation reservation)
    {
        dbContext.Reservations.Add(reservation);

        return dbContext.SaveChanges() > 0 ? reservation : null;
    }
}