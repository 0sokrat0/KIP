using pr7.Models;

namespace pr7.Repositories;

public interface IPartRepository
{
    Part GetById(int id);
    List<Part> GetAll();
    void Add(Part part);
    void Update(Part part);
    void Delete(int id);
}

