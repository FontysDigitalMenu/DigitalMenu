using DigitalMenu_20_BLL.Dtos;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

// using QRCoder;

namespace DigitalMenu_20_BLL.Services;

public class TableService(ITableRepository tableRepository, IReservationService reservationService) : ITableService
{
    // public string GenerateQrCode(string backendUrl, string id)
    // {
    //     QRCodeGenerator qrCodeGenerator = new();
    //     QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode($"{backendUrl}/table/{id}", QRCodeGenerator.ECCLevel.Q);
    //     BitmapByteQRCode bitmapByteQrCode = new(qrCodeData);
    //     byte[] qrCodeAsBitmapByte = bitmapByteQrCode.GetGraphic(20);
    //
    //     return Convert.ToBase64String(qrCodeAsBitmapByte);
    // }

    public List<Table> GetAll()
    {
        return tableRepository.GetAll();
    }

    public Table? Create(Table table)
    {
        return tableRepository.Create(table);
    }

    public Table? GetById(string id)
    {
        return tableRepository.GetById(id);
    }

    public Table? GetBySessionId(string sessionId)
    {
        return tableRepository.GetBySessionId(sessionId);
    }

    public bool Update(Table table)
    {
        return tableRepository.Update(table);
    }

    public bool Delete(string id)
    {
        return tableRepository.Delete(id);
    }

    public bool ResetSession(string id)
    {
        Table? table = tableRepository.GetById(id);

        if (table == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        table.HostId = null;
        table.SessionId = Guid.NewGuid().ToString();

        return tableRepository.Update(table);
    }

    public bool AddHost(string id, string deviceId)
    {
        Table? table = tableRepository.GetById(id);

        if (table == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        if (table.HostId == null)
        {
            table.HostId = deviceId;
            return tableRepository.Update(table);
        }

        return true;
    }

    public TableScan Scan(string id, int? code)
    {
        DateTime now = DateTime.Now;
        Table? table = tableRepository.GetTableByIdWithReservationsFromDay(id, now);

        if (table == null)
        {
            return new TableScan { Table = null };
        }

        Reservation? reservation = IsTableAvailableForAtLeastTwoAndAHalfHours(table, now);
        if (reservation == null)
        {
            return new TableScan { Table = table, IsReserved = false };
        }

        bool reservationIsHourOldAndNotUnlocked =
            reservation.ReservationDateTime.AddHours(1) <= now && !reservation.IsUnlocked;
        if (reservationIsHourOldAndNotUnlocked)
        {
            reservationService.Delete(reservation.Id);
            return new TableScan { Table = table, IsReserved = false };
        }

        if (reservation.ReservationId != code && !reservation.IsUnlocked)
        {
            return new TableScan { Table = table, IsReserved = true };
        }

        if (reservation.ReservationId == code)
        {
            reservationService.Unlock(reservation.Id);
        }

        return new TableScan { Table = table, IsReserved = true, IsUnlocked = true };
    }

    private static Reservation? IsTableAvailableForAtLeastTwoAndAHalfHours(Table table, DateTime dateTime)
    {
        foreach (Reservation reservation in table.Reservations)
        {
            if (reservation.ReservationDateTime.TimeOfDay <= dateTime.TimeOfDay &&
                reservation.ReservationDateTime.AddHours(ReservationService.ReservationDuration) >= dateTime)
            {
                return reservation;
            }
        }

        return null;
    }
}