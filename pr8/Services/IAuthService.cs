using pr8.Models;

namespace pr8.Services;

public interface IAuthService
{
    bool Register(string username, string password, string confirmPassword);
    User? Login(string username, string password);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

