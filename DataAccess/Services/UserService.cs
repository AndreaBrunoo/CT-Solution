using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class UserService : IUserService
{
    private readonly XpoDataContext _ctx;
    private readonly PasswordService _passwordService;
    private readonly JwtService _jwtService;

    public UserService(UnitOfWork uow, PasswordService passwordService, JwtService jwtService)
    {
        _ctx = new XpoDataContext(uow);
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task RegisterAsync(string email, string password, string role)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoUser>()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existing != null)
                throw new Exception("Email already registered");

            var (hash, salt) = _passwordService.HashPassword(password);

            var domain = new User(Guid.NewGuid(), email, hash, salt, role);

            XpoUserMapper.ToXpo(domain, uow);

            return true;
        });
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.Query<XpoUser>()
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new Exception("Invalid credentials");

            var domain = XpoUserMapper.ToDomain(xpo);

            var valid = _passwordService.VerifyPassword(password, domain.PasswordHash, domain.PasswordSalt);

            if (!valid)
                throw new Exception("Invalid credentials");

            return _jwtService.GenerateToken(domain);
        });
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoUser>(id);
            if (xpo == null) return null;

            var domain = XpoUserMapper.ToDomain(xpo);

            return XpoUserMapper.ToDto(domain);
        });
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.Query<XpoUser>()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (xpo == null) return null;

            var domain = XpoUserMapper.ToDomain(xpo);

            return XpoUserMapper.ToDto(domain);
        });
    }
}