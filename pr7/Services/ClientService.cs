using pr7.Models;
using pr7.Repositories;

namespace pr7.Services;

public class ClientService : IClientService
{
    private readonly IPartRepository _partRepository;
    private readonly ICarRepository _carRepository;
    private readonly IClientRepository _clientRepository;
    private readonly Random _random = new Random();
    
    private readonly string[] _clientNames = new[]
    {
        "Иван Петров", "Мария Сидорова", "Алексей Козлов", "Елена Новикова",
        "Дмитрий Волков", "Анна Соколова", "Сергей Лебедев", "Ольга Орлова",
        "Николай Морозов", "Татьяна Павлова"
    };
    
    private readonly string[] _carModels = new[]
    {
        "Toyota Camry", "Honda Accord", "BMW 3 Series", "Mercedes C-Class",
        "Audi A4", "Volkswagen Passat", "Ford Focus", "Hyundai Elantra",
        "Kia Optima", "Mazda 6"
    };
    
    public ClientService(
        IPartRepository partRepository,
        ICarRepository carRepository,
        IClientRepository clientRepository)
    {
        _partRepository = partRepository;
        _carRepository = carRepository;
        _clientRepository = clientRepository;
    }
    
    public Client GenerateRandomClient()
    {
        var brokenPart = GenerateRandomBrokenPart();
        var car = GenerateRandomCar(brokenPart.Id);
        
        var client = new Client
        {
            Name = _clientNames[_random.Next(_clientNames.Length)],
            CarId = car.Id
        };
        
        _clientRepository.Add(client);
        return client;
    }
    
    public Car GenerateRandomCar(int brokenPartId)
    {
        var car = new Car
        {
            Model = _carModels[_random.Next(_carModels.Length)],
            BrokenPartId = brokenPartId
        };
        
        _carRepository.Add(car);
        return car;
    }
    
    public Part GenerateRandomBrokenPart()
    {
        var allParts = _partRepository.GetAll();
        if (allParts.Count == 0)
            throw new InvalidOperationException("В базе данных нет деталей");
        
        var randomPart = allParts[_random.Next(allParts.Count)];
        return randomPart;
    }
}

