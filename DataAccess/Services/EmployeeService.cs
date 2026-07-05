using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class EmployeeService : IEmployeeService
{
    private readonly XpoDataContext _ctx;

    public EmployeeService(UnitOfWork uow)
    {
        _ctx = new XpoDataContext(uow);
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoEmployee>(id, cancellationToken);
            if (xpo == null) return null;

            var domain = XpoEmployeeMapper.ToDomain(xpo);

            return XpoEmployeeMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<EmployeeDto>?> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoEmployee>().ToListAsync(cancellationToken);

            if (list == null) return null;
            return list
            .Select(xpo =>
            {
                var domain = XpoEmployeeMapper.ToDomain(xpo);

                return XpoEmployeeMapper.ToDto(domain);
            })
            .ToList();
        });
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            // Controllo duplicati
            var existing = await uow.Query<XpoEmployee>()
                .FirstOrDefaultAsync(e =>
                e.UserName == dto.UserName &&
                e.User.Id == dto.IdUser,
                cancellationToken);

            if (existing != null)
                throw new Exception("Employee already exists");

            // Domain
            var domain = new Employee(
                id: Guid.NewGuid(),
                userName: dto.UserName,
                idUser: dto.IdUser
            );

            // XPO
            var xpo = XpoEmployeeMapper.ToXpo(domain, uow);

            // DTO
            return XpoEmployeeMapper.ToDto(domain);
        });
    }

    public async Task<EmployeeDto?> UpdateAsync(UpdateEmployeeDto dto, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            // 1. Carico l'XPO esistente tramite ID
            var xpo = await uow.GetObjectByKeyAsync<XpoEmployee>(dto.Id, cancellationToken);
            if (xpo == null)
                throw new Exception("Employee not found");

            // 2. Converto XPO → Domain
            var domain = XpoEmployeeMapper.ToDomain(xpo);

            // 3. Aggiorno il Domain con i valori del DTO
            domain.UserName = dto.UserName;

            // 4. Aggiorno l’XPO tramite il mapper
            XpoEmployeeMapper.ToXpo(domain, uow);

            // 5. Restituisco il DTO di output
            return XpoEmployeeMapper.ToDto(domain);
        });
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoEmployee>(id, cancellationToken);
            if (xpo == null)
                throw new Exception("Employee not found");

            xpo.Delete();

            return true;
        });
    }
}