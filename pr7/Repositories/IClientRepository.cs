using pr7.Models;

namespace pr7.Repositories;

public interface IClientRepository
{
    Client GetById(int id);
    List<Client> GetAll();
    void Add(Client client);
    void Update(Client client);
    void Delete(int id);
}

