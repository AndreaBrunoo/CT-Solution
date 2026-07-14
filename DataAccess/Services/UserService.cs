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

    public async Task RegisterAsync(string email, string password)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoUser>()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existing != null)
                throw new Exception("Email already registered");

            var (hash, salt) = _passwordService.HashPassword(password);

            var xpoRole = await uow.Query<XpoRole>()
                .FirstOrDefaultAsync(r => r.Name == "User");

            if (xpoRole == null)
            {
                xpoRole = new XpoRole(uow)
                {
                    Id = Guid.NewGuid(),
                    Name = "User"
                };
            }

            var domain = new User(
                Guid.NewGuid(),
                email,
                hash,
                salt,
                new List<Role>
                {
                new Role(xpoRole.Id, xpoRole.Name)
                }
            );

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

    public async Task<IReadOnlyList<UserDto>?> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoUser>().ToListAsync(ct);

            return list
            .Select(xpo =>
            {
                var domain = XpoUserMapper.ToDomain(xpo);

                return XpoUserMapper.ToDto(domain);
            })
            .ToList();
        });
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoUser>(id, ct);

            return xpo is null
                ? null
                : XpoUserMapper.ToDto(XpoUserMapper.ToDomain(xpo));
        });
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.Query<XpoUser>()
                .FirstOrDefaultAsync(u => u.Email == email, ct);

            return xpo is null
                ? null
                : XpoUserMapper.ToDto(XpoUserMapper.ToDomain(xpo));
        });
    }

    // ---------------------------------------------------------
    // ASSIGN ROLE
    // ---------------------------------------------------------
    public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var user = await uow.GetObjectByKeyAsync<XpoUser>(userId, ct)
                ?? throw new Exception("User not found.");

            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct)
                ?? throw new Exception("Role not found.");

            if (!user.Roles.Contains(role))
                user.Roles.Add(role);

            return true;
        });
    }

    // ---------------------------------------------------------
    // REMOVE ROLE
    // ---------------------------------------------------------
    public async Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var user = await uow.GetObjectByKeyAsync<XpoUser>(userId, ct)
                ?? throw new Exception("User not found.");

            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct)
                ?? throw new Exception("Role not found.");

            if (user.Roles.Contains(role))
                user.Roles.Remove(role);

            return true;
        });
    }

    // ---------------------------------------------------------
    // CHECK ROLE
    // ---------------------------------------------------------
    public async Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var user = await uow.GetObjectByKeyAsync<XpoUser>(userId, ct);
            if (user == null) return false;

            return user.Roles.Any(r => r.Name == roleName);
        });
    }

    // ---------------------------------------------------------
    // CHECK PERMISSION
    // ---------------------------------------------------------
    public async Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var user = await uow.GetObjectByKeyAsync<XpoUser>(userId, ct);
            if (user == null) return false;

            return user.Roles
                .SelectMany(r => r.Permissions)
                .Any(p => p.Code == permissionCode);
        });
    }
}