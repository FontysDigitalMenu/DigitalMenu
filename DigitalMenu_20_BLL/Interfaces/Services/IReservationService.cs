using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IReservationService
{
    public Reservation? Create(Reservation reservation);
}