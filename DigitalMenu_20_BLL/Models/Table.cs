namespace DigitalMenu_20_BLL.Models;

public class Table
{
    public string Id { get; set; }

    public string Name { get; set; }

    public List<Order> Orders { get; set; }

    public string SessionId { get; set; }

    public string? HostId { get; set; }

    public string? QrCode { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsReservable { get; set; }

    public List<Reservation> Reservations { get; set; }
}