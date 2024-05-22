namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IEmailService
{
    public void SendReservationEmail(string toEmail, string reservationCode, string tableName, string language);
}