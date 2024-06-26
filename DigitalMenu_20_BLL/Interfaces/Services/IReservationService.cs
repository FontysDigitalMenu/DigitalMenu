﻿using DigitalMenu_20_BLL.Dtos;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IReservationService
{
    public Reservation? Create(Reservation reservation, string language);

    public List<Reservation> GetByDay(DateTime dateTime);

    public List<AvailableTimes> GetAvailableTimes(DateTime date);

    public void Delete(string reservationId);

    public void Unlock(string reservationId);

    public bool MustPayReservationFee(string tableSessionId);

    public List<Reservation> GetReservations(DateTime dateTime);

    public Reservation? GetReservationById(string reservationId);
}