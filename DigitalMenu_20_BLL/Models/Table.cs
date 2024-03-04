namespace DigitalMenu_20_BLL.Models;

public class Table
{
    public int Id { get; set; }

    public string QRCode { get; set; }

    public List<Order> Orders { get; set; }
}