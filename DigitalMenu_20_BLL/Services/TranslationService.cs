using System.Text;
using DigitalMenu_20_BLL.Interfaces.Services;
using Newtonsoft.Json;

namespace DigitalMenu_20_BLL.Services;

public class TranslationService(string apiUrl) : ITranslationService
{
    public static readonly List<string> SupportedLanguages = ["en", "nl", "de", "ko"];

    private static readonly HttpClient Client = new();

    public async Task<string> Translate(string text, string sourceLanguage, string targetLanguage)
    {
        var request = new
        {
            q = text,
            source = sourceLanguage,
            target = targetLanguage,
            format = "text",
        };

        string json = JsonConvert.SerializeObject(request);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await Client.PostAsync($"{apiUrl}/translate", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Translation failed: {response.StatusCode}");
        }

        string responseContent = await response.Content.ReadAsStringAsync();
        var translationResponse = JsonConvert.DeserializeAnonymousType(responseContent, new { translatedText = "" });

        return translationResponse!.translatedText;
    }
}