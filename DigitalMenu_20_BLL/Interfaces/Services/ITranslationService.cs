namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface ITranslationService
{
    public Task<string> Translate(string text, string sourceLanguage, string targetLanguage);
}