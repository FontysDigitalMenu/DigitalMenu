using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface ISplitRepository
{
    public Split? GetByExternalPaymentId(string id);

    public bool Update(Split split);

    public Split? GetById(int id);

    public Split? Create(Split split);

    public bool CreateSplits(List<Split> splits);
}