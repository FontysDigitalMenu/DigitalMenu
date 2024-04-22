namespace DigitalMenu_10_Api.RequestModels;

public class SplitRequest
{
    public string OrderId { get; set; }

    public string Name { get; set; }

    public int Amount { get; set; }

    public string ExternalPaymentId { get; set; }
}