using pr7.Models;

namespace pr7.Services;

public interface IWarehouseService
{
    bool HasPart(int partId, int quantity = 1);
    void AddPart(int partId, int quantity);
    void RemovePart(int partId, int quantity = 1);
    List<WarehouseItem> GetAvailableParts();
    List<WarehouseItem> GetPendingDeliveries();
    void ProcessDeliveries(int carsProcessed);
    void ScheduleDelivery(int partId, int quantity);
    int GetRandomAvailablePartId();
}

