using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Newtonsoft.Json;

namespace DigitalMenu_20_BLL.Services;

public class EmailService(
    IMailTranslationRepository mailTranslationRepository,
    string fromAddress,
    string fromName,
    string password,
    int port,
    string host) : IEmailService
{
    public void SendReservationEmail(string toEmail, string reservationCode, string tableName, string language)
    {
        MailTranslation mailTranslation =
            mailTranslationRepository.GetMailTranslationBy("reservation-created", language);

        var translationResponse = JsonConvert.DeserializeAnonymousType(mailTranslation.Body, new
        {
            title = "",
            salutation = "",
            instruction = "",
            thankYou = "",
            bestRegards = "",
            companyName = "",
        });


        string body = $@"
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ccc; border-radius: 10px;'>
                        <h2 style='color: #2E86C1; text-align: center;'>{translationResponse!.title}</h2>
                        <p style='font-size: 16px;'>{translationResponse.salutation}</p>
                        <p style='font-size: 16px;'>{translationResponse.instruction}</p>
                        <p style='font-size: 24px; font-weight: bold; text-align: center; color: #27AE60;'>{reservationCode}</p>
                        <p style='font-size: 24px; font-weight: bold; text-align: center;'>{tableName}</p>
                        <p style='font-size: 16px;'>{translationResponse.thankYou}</p>
                        <p style='font-size: 16px;'>{translationResponse.bestRegards}</p>
                        <p style='font-size: 16px;'>{translationResponse.companyName}</p>
                    </div>
                </body>
            </html>";

        SendEmail(toEmail, mailTranslation.Subject, body);
    }

    public void SendEmail(string toEmail, string subject, string body)
    {
        MailAddress from = new(fromAddress, fromName);
        MailAddress to = new(toEmail);

        SmtpClient smtp = new()
        {
            Host = host,
            Port = port,
            EnableSsl = false,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(from.Address, password),
        };

        using MailMessage message = new(from, to);

        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;
        smtp.Send(message);
    }

    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

            string DomainMapper(Match match)
            {
                IdnMapping idn = new();
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase,
                TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}