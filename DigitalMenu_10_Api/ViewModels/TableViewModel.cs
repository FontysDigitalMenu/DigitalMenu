namespace DigitalMenu_10_Api.ViewModels;

public class TableViewModel
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string QrCode { get; set; }

    public bool IsReservable { get; set; }

    public string SessionId { get; set; }
}