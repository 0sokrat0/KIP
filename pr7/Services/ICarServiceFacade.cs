using pr7.Models;

namespace pr7.Services;

public interface ICarServiceFacade
{
    void ProcessNewClient();
    void AcceptOrder();
    void RejectOrder();
    void ShowWarehouse();
    void ShowShop();
    void BuyPart(int partId, int quantity);
    void ShowBalance();
    void ShowStatistics();
    decimal GetBalance();
    int GetCarsProcessed();
    bool HasPendingOrder { get; }
    Order? CurrentOrder { get; }
    event EventHandler<string[]>? OutputChanged;
    event EventHandler<string>? BalanceChanged;
}

