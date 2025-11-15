using pr8.Models;

namespace pr8.Repositories;

public interface IUserRepository
{
    User? GetByUsername(string username);
    User? GetById(int id);
    void Add(User user);
    bool Exists(string username);
}

