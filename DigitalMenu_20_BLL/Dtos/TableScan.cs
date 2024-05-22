using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Dtos;

public class TableScan
{
    public Table? Table { get; set; }

    public bool IsReserved { get; set; }

    public bool IsUnlocked { get; set; }
}