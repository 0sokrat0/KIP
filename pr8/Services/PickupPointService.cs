using pr8.Models;
using pr8.Repositories;

namespace pr8.Services;

public class PickupPointService : IPickupPointService
{
    private readonly IPickupPointRepository _pickupPointRepository;
    
    public PickupPointService(IPickupPointRepository pickupPointRepository)
    {
        _pickupPointRepository = pickupPointRepository;
    }
    
    public List<PickupPoint> GetAllPickupPoints()
    {
        return _pickupPointRepository.GetAll();
    }
    
    public PickupPoint? GetPickupPointById(int id)
    {
        return _pickupPointRepository.GetById(id);
    }
}

