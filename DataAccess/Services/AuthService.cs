using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class UserService : IUserService
{
    private readonly UnitOfWork _uow;
    private readonly PasswordService _passwordService;
    private readonly JwtService _jwtService;

    public UserService(UnitOfWork uow, PasswordService passwordService, JwtService jwtService)
    {
        _uow = uow;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task RegisterAsync(string email, string password, string role)
    {
        var existing = await GetByEmailAsync(email);
        if (existing != null)
            throw new Exception("Email already registered");

        var (hash, salt) = _passwordService.HashPassword(password);

        var domain = new User(
            Guid.NewGuid(),
            email,
            hash,
            salt,
            role);

        var xpo = XpoUserMapper.ToXpo(domain, _uow);
        await _uow.CommitChangesAsync();
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await GetDomainByEmailAsync(email)
                   ?? throw new Exception("Invalid credentials");

        var valid = _passwordService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);

        if (!valid)
            throw new Exception("Invalid credentials");

        return _jwtService.GenerateToken(user);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var xpo = await _uow.GetObjectByKeyAsync<XpoUser>(id);
        if (xpo == null) return null;

        var domain = XpoUserMapper.ToDomain(xpo);

        return new UserDto
        {
            Id = domain.Id,
            Email = domain.Email,
            Role = domain.Role
        };
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var xpo = await _uow.Query<XpoUser>()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (xpo == null) return null;

        var domain = XpoUserMapper.ToDomain(xpo);

        return new UserDto
        {
            Id = domain.Id,
            Email = domain.Email,
            Role = domain.Role
        };
    }
    private async Task<User?> GetDomainByEmailAsync(string email)
    {
        var xpo = await _uow.Query<XpoUser>()
            .FirstOrDefaultAsync(u => u.Email == email);

        return xpo == null ? null : XpoUserMapper.ToDomain(xpo);
    }
}