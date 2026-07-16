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
    private readonly IActionLogger _logger;


    public UserService(UnitOfWork uow, PasswordService passwordService, JwtService jwtService, IActionLogger logger)
    {
        _ctx = new XpoDataContext(uow);
        _passwordService = passwordService;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task RegisterAsync(RegisterDto dto, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoUser>()
                .FirstOrDefaultAsync(u => u.Email == dto.Email, ct);

            if (existing != null)
            {
                await _logger.LogFailureAsync("Regsiter", "User", null,
                    $"User '{dto.Email}' already exists", ct);
                throw new Exception("User already exists");
            }

            var (hash, salt) = _passwordService.HashPassword(dto.Password);

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
                dto.Email,
                hash,
                salt,
                new List<Role>
                {
                    new Role(xpoRole.Id, xpoRole.Name)
                }
            );

            XpoUserMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Register", "User", domain.Id, ct);

            return true;
        });
    }


    public async Task<string> LoginAsync(LoginDto dto, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.Query<XpoUser>()
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (xpo == null)
            {
                await _logger.LogFailureAsync("Login", "User", null,
                    $"User '{dto.Email}' not exists", ct);

                throw new Exception("User not exists");
            }

            var domain = XpoUserMapper.ToDomain(xpo);

            var valid = _passwordService.VerifyPassword(dto.Password, domain.PasswordHash, domain.PasswordSalt);
            if (!valid)
                throw new Exception("Invalid credentials");

            if (domain.Employee == null)
            {
                var employee = new Employee(
                    Guid.NewGuid(),
                    domain.Email
                );

                employee.IdUser = domain.Id;
                domain.Employee = employee;

                var xpoEmployee = XpoEmployeeMapper.ToXpo(employee, uow);

                await uow.CommitChangesAsync();
            }

            await _logger.LogSuccessAsync(uow, "Login", "User", domain.Id, ct);

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
            var user = await uow.GetObjectByKeyAsync<XpoUser>(userId, ct);
            if (user == null)
            {
                await _logger.LogFailureAsync("AssignRole", "User", null,
                    $"User '{roleId}' User not found", ct);
                throw new Exception("User not found");
            }

            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct);
            if (role == null)
            {
                await _logger.LogFailureAsync("AssignRole", "Role", null,
                    $"Role '{roleId}' Role not found", ct);
                throw new Exception("Role not found");
            }

            if (!user.Roles.Contains(role))
                user.Roles.Add(role);

            await _logger.LogSuccessAsync(uow, "AssignRole", "User", userId, ct);

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
            var user = await uow.GetObjectByKeyAsync<XpoUser>(userId, ct);
            if (user == null)
            {
                await _logger.LogFailureAsync("RemoveRole", "User", null,
                    $"User '{roleId}' User not found", ct);
                throw new Exception("User not found");
            }

            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct);
            if (role == null)
            {
                await _logger.LogFailureAsync("RemoveRole", "Role", null,
                    $"Role '{roleId}' Role not found", ct);
                throw new Exception("Role not found");
            }

            if (user.Roles.Contains(role))
                user.Roles.Remove(role);

            await _logger.LogSuccessAsync(uow, "RemoveRole", "User", userId, ct);

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
    // UPDATE PASSWORD
    // ---------------------------------------------------------
    public async Task UpdatePasswordAsync(Guid userId, UpdatePasswordDto dto, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoUser>(userId, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("UpdatePassword", "User", null,
                    $"User '{userId}' not found", ct);
                throw new Exception("User not found");
            }

            var domain = XpoUserMapper.ToDomain(xpo);

            if (!_passwordService.VerifyPassword(dto.CurrentPassword, domain.PasswordHash, domain.PasswordSalt))
            {
                await _logger.LogFailureAsync("UpdatePassword", "User", userId,
                    "Current password is invalid", ct);
                throw new Exception("Invalid current password");
            }

            var (newHash, newSalt) = _passwordService.HashPassword(dto.NewPassword);

            xpo.PasswordHash = newHash;
            xpo.PasswordSalt = newSalt;

            await _logger.LogSuccessAsync(uow, "UpdatePassword", "User", userId, ct);

            return true;
        });
    }

    // ---------------------------------------------------------
    // DELETE USER
    // ---------------------------------------------------------
    public async Task DeleteAsync(Guid userId, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoUser>(userId, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("Delete", "User", null,
                    $"User '{userId}' not found", ct);
                throw new Exception("User not found");
            }

            xpo.Roles.Reload();
            foreach (var role in xpo.Roles.ToList())
                xpo.Roles.Remove(role);

            await uow.DeleteAsync(xpo, ct);

            await _logger.LogSuccessAsync(uow, "Delete", "User", userId, ct);

            return true;
        });
    }
}