namespace DigitalMenu_20_BLL.Interfaces;

public interface IOrderService
{
    public int GetTotalAmount();
    
    public bool Create(string deviceId, string tableId, string paymentId, int totalAmount);
}