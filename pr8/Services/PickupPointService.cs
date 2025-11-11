using pr8.Models;
using pr8.Repositories;

namespace pr8.Services;

public class PickupPointService
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
    
    public void DisplayPickupPoints(List<PickupPoint> pickupPoints)
    {
        if (pickupPoints.Count == 0)
        {
            Console.WriteLine("ПВЗ не найдены");
            return;
        }
        
        Console.WriteLine("ПУНКТЫ ВЫДАЧИ ЗАКАЗОВ");
        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"{"ID",-5} {"Название",-30} {"Адрес"}");
        Console.WriteLine(new string('-', 80));
        
        foreach (var pp in pickupPoints)
        {
            Console.WriteLine($"{pp.Id,-5} {pp.Name,-30} {pp.Address}");
        }
        
        Console.WriteLine(new string('-', 80));
    }
}
