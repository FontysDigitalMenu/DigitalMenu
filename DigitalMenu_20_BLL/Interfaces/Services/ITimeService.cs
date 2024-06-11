using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface ITimeService
{
    public DateTime GetNow();

    public void UpdateOrCreate(Time time);

    public void Delete();
}