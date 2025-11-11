using pr8.Models;

namespace pr8.Repositories;

public interface IUserRepository
{
    User? GetById(int id);
    User? GetByUsername(string username);
    List<User> GetAll();
    void Add(User user);
    void Update(User user);
    void Delete(int id);
}

