using System.Security.Cryptography;
using System.Text;
using pr8.Models;
using pr8.Repositories;

namespace pr8.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public bool Register(string username, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Имя пользователя не может быть пустым");
        
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Пароль не может быть пустым");
        
        if (password != confirmPassword)
            throw new ArgumentException("Пароли не совпадают");
        
        if (_userRepository.Exists(username))
            throw new InvalidOperationException("Пользователь с таким именем уже существует");
        
        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password)
        };
        
        _userRepository.Add(user);
        return true;
    }
    
    public User? Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;
        
        var user = _userRepository.GetByUsername(username);
        if (user == null)
            return null;
        
        if (!VerifyPassword(password, user.PasswordHash))
            return null;
        
        return user;
    }
    
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
    
    public bool VerifyPassword(string password, string hash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == hash;
    }
}

