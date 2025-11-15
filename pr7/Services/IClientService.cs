using pr7.Models;

namespace pr7.Services;

public interface IClientService
{
    Client GenerateRandomClient();
    Car GenerateRandomCar(int brokenPartId);
    Part GenerateRandomBrokenPart();
}

