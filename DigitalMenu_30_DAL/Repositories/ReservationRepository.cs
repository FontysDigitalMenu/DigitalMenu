﻿using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

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

    public List<Reservation> GetByDay(DateTime dateTime)
    {
        return dbContext.Reservations.Where(r => r.ReservationDateTime.Date == dateTime.Date).ToList();
    }

    public void Delete(string reservationId)
    {
        Reservation? reservation = dbContext.Reservations.FirstOrDefault(r => r.Id == reservationId);
        if (reservation == null)
        {
            return;
        }

        dbContext.Reservations.Remove(reservation);
        dbContext.SaveChanges();
    }

    public void Unlock(string id)
    {
        Reservation? reservation = dbContext.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null)
        {
            return;
        }

        reservation.IsUnlocked = true;
        dbContext.SaveChanges();
    }

    public List<Reservation> GetReservations(DateTime dateTime)
    {
        return dbContext.Reservations.Include(r => r.Table).Where(r => r.ReservationDateTime.Date == dateTime.Date)
            .ToList();
    }

    public Reservation? GetReservationById(string reservationId)
    {
        return dbContext.Reservations.Include(r => r.Table).FirstOrDefault(r => r.Id == reservationId);
    }
}