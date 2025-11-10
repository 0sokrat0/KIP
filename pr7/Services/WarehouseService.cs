using pr7.Models;

namespace pr7.Services;

public class WarehouseService
{
    public bool HasPart(int partId, int quantity) { return false; }
    
    public void AddPart(int partId, int quantity) { }
    
    public void RemovePart(int partId, int quantity) { }
    
    public List<WarehouseItem> GetAvailableParts() { return new List<WarehouseItem>(); }
    
    public List<WarehouseItem> GetPendingDeliveries() { return new List<WarehouseItem>(); }
    
    public void ProcessDeliveries(int carsProcessed) { }
}

