using pr8.Models;
using pr8.Repositories;

namespace pr8.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public bool Register(string username, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Имя пользователя не может быть пустым");
            return false;
        }
        
        if (!ValidatePassword(password, confirmPassword))
        {
            return false;
        }
        
        if (IsUsernameTaken(username))
        {
            Console.WriteLine("Пользователь с таким именем уже существует");
            return false;
        }
        
        var user = new User
        {
            Username = username,
            Password = password
        };
        
        try
        {
            _userRepository.Add(user);
            Console.WriteLine("Регистрация успешна!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при регистрации: {ex.Message}");
            return false;
        }
    }
    
    public User? Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Имя пользователя и пароль не могут быть пустыми");
            return null;
        }
        
        var user = _userRepository.GetByUsername(username);
        if (user == null)
        {
            Console.WriteLine("Пользователь с таким именем не найден");
            return null;
        }
        
        if (user.Password != password)
        {
            Console.WriteLine("Неверный пароль");
            return null;
        }
        
        Console.WriteLine($"Добро пожаловать, {user.Username}!");
        return user;
    }
    
    public bool ValidatePassword(string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Пароль не может быть пустым");
            return false;
        }
        
        if (password != confirmPassword)
        {
            Console.WriteLine("Пароли не совпадают");
            return false;
        }
        
        return true;
    }
    
    public bool IsUsernameTaken(string username)
    {
        return _userRepository.GetByUsername(username) != null;
    }
}
