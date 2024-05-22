using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IMailTranslationRepository
{
    public MailTranslation? GetMailTranslationBy(string type, string language);
}