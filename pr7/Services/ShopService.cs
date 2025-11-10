using pr7.Models;

namespace pr7.Services;

public class ShopService
{
    public List<Part> GetAvailablePartsForPurchase() { return new List<Part>(); }
    
    public bool BuyParts(int partId, int quantity, decimal balance) { return false; }
    
    public void ScheduleDelivery(int partId, int quantity) { }
}

