using pr7.Models;

namespace pr7.Services;

public interface IShopService
{
    List<Part> GetAvailablePartsForPurchase();
    bool BuyParts(int partId, int quantity);
}

