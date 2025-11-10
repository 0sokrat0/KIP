using pr7.Models;

namespace pr7.Repositories;

public interface ICarRepository
{
    Car GetById(int id);
    List<Car> GetAll();
    void Add(Car car);
    void Update(Car car);
    void Delete(int id);
}

