using pr7.Models;

namespace pr7.Repositories;

public interface IWarehouseRepository
{
    WarehouseItem? GetByPartId(int partId);
    List<WarehouseItem> GetAll();
    List<WarehouseItem> GetPendingDeliveries();
    void Add(WarehouseItem item);
    void Update(WarehouseItem item);
    void Delete(int id);
}
