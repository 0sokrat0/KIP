using pr8.Models;

namespace pr8.Services;

public interface IPickupPointService
{
    List<PickupPoint> GetAllPickupPoints();
    PickupPoint? GetPickupPointById(int id);
}

