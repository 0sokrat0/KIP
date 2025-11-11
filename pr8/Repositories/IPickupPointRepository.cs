using pr8.Models;

namespace pr8.Repositories;

public interface IPickupPointRepository
{
    PickupPoint? GetById(int id);
    List<PickupPoint> GetAll();
    void Add(PickupPoint pickupPoint);
    void Update(PickupPoint pickupPoint);
    void Delete(int id);
}

