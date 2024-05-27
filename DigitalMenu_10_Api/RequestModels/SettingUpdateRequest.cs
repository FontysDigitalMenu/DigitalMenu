namespace DigitalMenu_10_Api.RequestModels;

public class SettingUpdateRequest
{
    public int Id { get; set; }

    public string CompanyName { get; set; }

    public string PrimaryColor { get; set; }

    public string SecondaryColor { get; set; }
}