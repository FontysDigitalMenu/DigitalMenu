using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class DateTimeService(ITimeRepository timeRepository) : ITimeService
{
    public DateTime GetNow()
    {
        Time? time = timeRepository.Get();

        if (time != null)
        {
            return DateTime.Today.AddHours(time.Hours).AddMinutes(time.Minutes);
        }

        return DateTime.Now;
    }

    public void UpdateOrCreate(Time time)
    {
        timeRepository.UpdateOrCreate(time);
    }

    public void Delete()
    {
        timeRepository.Delete();
    }
}