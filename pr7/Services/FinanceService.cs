using pr7.Repositories;

namespace pr7.Services;

public class FinanceService
{
    private readonly ICarServiceRepository _carServiceRepository;
    private const decimal PENALTY_AMOUNT = 500.00m;
    private const decimal WORK_COST_MULTIPLIER = 0.3m;
    
    public FinanceService(ICarServiceRepository carServiceRepository)
    {
        _carServiceRepository = carServiceRepository;
    }
    
    public void AddMoney(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Сумма не может быть отрицательной");
        
        var carService = _carServiceRepository.Get();
        carService.Balance += amount;
        _carServiceRepository.Update(carService);
    }
    
    public void SubtractMoney(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Сумма не может быть отрицательной");
        
        if (!HasEnoughMoney(amount))
            throw new InvalidOperationException("Недостаточно средств");
        
        var carService = _carServiceRepository.Get();
        carService.Balance -= amount;
        _carServiceRepository.Update(carService);
    }
    
    public bool HasEnoughMoney(decimal amount)
    {
        if (amount < 0)
            return false;
        
        var carService = _carServiceRepository.Get();
        return carService.Balance >= amount;
    }
    
    public decimal GetBalance()
    {
        var carService = _carServiceRepository.Get();
        return carService.Balance;
    }
    
    public void PayPenalty(decimal amount = PENALTY_AMOUNT)
    {
        SubtractMoney(amount);
    }
    
    public decimal CalculateWorkCost(decimal partPrice)
    {
        return partPrice * WORK_COST_MULTIPLIER;
    }
}

