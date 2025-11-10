using pr7.Models;

namespace pr7.Services;

public class ClientService
{
    public Client GenerateRandomClient() { return new Client(); }
    
    public Car GenerateRandomCar() { return new Car(); }
    
    public Part GenerateRandomBrokenPart() { return new Part(); }
}

