﻿using DigitalMenu_20_BLL.Dtos;
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
    private const double ReservationDuration = 2.5;

    private readonly List<string> _validReservationTimes = ["13:00", "15:30", "18:00", "20:30", "23:00"];

    public Reservation? Create(Reservation reservation, string language)
    {
        ValidateReservationBeforeCreation(reservation);
        reservation.ReservationId = new Random().Next(100000, 999999);
        reservation.ReservationDateTime = TruncateToNearestMinute(reservation.ReservationDateTime);

        Table availableTable = GetAvailableTable(reservation.ReservationDateTime);
        reservation.Table = availableTable;
        reservation.TableId = availableTable.Id;

        Reservation? createdReservation = reservationRepository.Create(reservation);
        if (createdReservation == null)
        {
            throw new ReservationException("Failed to create reservation");
        }

        emailService.SendReservationEmail(reservation.Email, reservation.ReservationId.ToString(),
            reservation.Table.Name, language);

        return reservation;
    }

    public List<Reservation> GetByDay(DateTime dateTime)
    {
        return reservationRepository.GetByDay(dateTime);
    }

    public List<AvailableTimes> GetAvailableTimes(DateTime date)
    {
        List<Reservation> reservations = reservationRepository.GetByDay(date);

        List<AvailableTimes> availableTimes = [];
        foreach (string validReservationTime in _validReservationTimes)
        {
            DateTime validTime = DateTime.Parse(validReservationTime);
            bool timeIsAvailable = reservations.All(r => r.ReservationDateTime.TimeOfDay != validTime.TimeOfDay);
            if (timeIsAvailable)
            {
                availableTimes.Add(new AvailableTimes
                {
                    startDateTime = date.Add(validTime.TimeOfDay),
                    endDateTime = date.Add(validTime.TimeOfDay).AddHours(ReservationDuration),
                });
            }
        }

        return availableTimes;
    }

    private Table GetAvailableTable(DateTime dateTime)
    {
        List<Table> tables = tableRepository.GetAllReservableTablesWithReservationsFrom(dateTime);

        DateTime reservationStart = dateTime;
        DateTime reservationEnd = reservationStart.AddHours(ReservationDuration);

        List<Table> availableTables = tables.Where(t => t.Reservations.All(r =>
            (reservationStart < r.ReservationDateTime && reservationEnd <= r.ReservationDateTime) ||
            reservationStart > r.ReservationDateTime
        )).ToList();

        if (availableTables.Count <= 0)
        {
            throw new ReservationException("No table is available");
        }

        return availableTables.First();
    }

    private static DateTime TruncateToNearestMinute(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
    }

    private void ValidateReservationBeforeCreation(Reservation reservation)
    {
        bool isValidTime = _validReservationTimes.Contains(reservation.ReservationDateTime.ToString("HH:mm"));
        if (!isValidTime)
        {
            throw new ReservationException("Invalid time");
        }

        bool isTwoAndAHalfHoursAhead = reservation.ReservationDateTime > DateTime.Now.AddHours(2.5);
        if (!isTwoAndAHalfHoursAhead)
        {
            throw new ReservationException("Reservation must be at least 2.5 hours ahead");
        }

        if (!EmailService.IsValid(reservation.Email))
        {
            throw new ReservationException("Invalid email");
        }
    }
}