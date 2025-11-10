using pr7.Models;

namespace pr7.Repositories;

public interface ICarServiceRepository
{
    CarService Get();
    void Update(CarService carService);
}

