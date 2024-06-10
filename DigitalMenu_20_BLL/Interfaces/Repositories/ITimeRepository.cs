using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface ITimeRepository
{
    public Time? Get();

    public bool Delete();

    public bool UpdateOrCreate(Time time);
}