namespace pr7.Services;

public interface IFinanceService
{
    void AddMoney(decimal amount);
    void SubtractMoney(decimal amount);
    bool HasEnoughMoney(decimal amount);
    decimal GetBalance();
    void PayPenalty(decimal amount = 500.00m);
    decimal CalculateWorkCost(decimal partPrice);
}

