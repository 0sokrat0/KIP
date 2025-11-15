using pr7.Models;
using pr7.Repositories;

namespace pr7.Services;

public class ShopService : IShopService
{
    private readonly IPartRepository _partRepository;
    private readonly IWarehouseService _warehouseService;
    private readonly IFinanceService _financeService;
    
    public ShopService(
        IPartRepository partRepository,
        IWarehouseService warehouseService,
        IFinanceService financeService)
    {
        _partRepository = partRepository;
        _warehouseService = warehouseService;
        _financeService = financeService;
    }
    
    public List<Part> GetAvailablePartsForPurchase()
    {
        return _partRepository.GetAll();
    }
    
    public bool BuyParts(int partId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля");
        
        var part = _partRepository.GetById(partId);
        var totalCost = part.Price * quantity;
        
        if (!_financeService.HasEnoughMoney(totalCost))
            return false;
        
        _financeService.SubtractMoney(totalCost);
        _warehouseService.ScheduleDelivery(partId, quantity);
        
        return true;
    }
}

