namespace DigitalMenu_10_Api.RequestModels;

public class OrderRequest
{
    public List<SplitRequest> Splits { get; set; }

    public string DeviceId { get; set; }

    public string TableId { get; set; }
}