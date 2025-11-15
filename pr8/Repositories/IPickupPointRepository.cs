using pr8.Models;

namespace pr8.Repositories;

public interface IPickupPointRepository
{
    PickupPoint? GetById(int id);
    List<PickupPoint> GetAll();
}

