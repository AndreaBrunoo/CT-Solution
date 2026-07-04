using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;
public interface IUserService
{
    Task RegisterAsync(string email, string password, string role);
    Task<string> LoginAsync(string email, string password);
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
}