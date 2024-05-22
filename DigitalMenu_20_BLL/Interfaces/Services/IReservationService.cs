using DigitalMenu_20_BLL.Dtos;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IReservationService
{
    public Reservation? Create(Reservation reservation);

    public List<Reservation> GetByDay(DateTime dateTime);

    public List<AvailableTimes> GetAvailableTimes(DateTime date);
}