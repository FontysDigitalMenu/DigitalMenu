namespace DigitalMenu_20_BLL.Models;

public class Reservation
{
    public int Id { get; set; }

    public int ReservationId { get; set; }

    public string Email { get; set; }

    public DateTime ReservationDateTime { get; set; }

    public string TableId { get; set; }

    public Table Table { get; set; }
    
    public bool IsUnlocked { get; set; }
}