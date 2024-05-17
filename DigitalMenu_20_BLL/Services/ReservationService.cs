using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class ReservationService(
    IReservationRepository reservationRepository,
    ITableRepository tableRepository,
    IEmailService emailService) : IReservationService
{
    private readonly List<string> _validReservationTimes = ["13:00", "15:30", "18:00", "20:30", "23:00"];

    public Reservation? Create(Reservation reservation)
    {
        ValidateReservationBeforeCreation(reservation);

        reservation.ReservationId = new Random().Next(100000, 999999);

        Reservation? createdReservation = reservationRepository.Create(reservation);
        if (createdReservation == null)
        {
            throw new ReservationException("Failed to create reservation");
        }

        emailService.SendReservationEmail(reservation.Email, reservation.ReservationId.ToString());

        return reservation;
    }

    private void ValidateReservationBeforeCreation(Reservation reservation)
    {
        bool isValidTime = _validReservationTimes.Contains(reservation.ReservationDateTime.ToString("HH:mm"))
                           && reservation.ReservationDateTime > DateTime.Now;
        if (!isValidTime)
        {
            throw new ReservationException("Invalid time");
        }

        bool isValidTable = tableRepository.GetById(reservation.TableId) != null;
        if (!isValidTable)
        {
            throw new ReservationException("Invalid table");
        }

        bool isTableAvailable =
            reservationRepository.GetBy(reservation.TableId, reservation.ReservationDateTime) == null;
        if (!isTableAvailable)
        {
            throw new ReservationException("Table is not available");
        }

        if (!EmailService.IsValid(reservation.Email))
        {
            throw new ReservationException("Invalid email");
        }
    }
}