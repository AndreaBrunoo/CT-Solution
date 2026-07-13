using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class CompanyService : ICompanyService
{
    private readonly XpoDataContext _ctx;

    public CompanyService(UnitOfWork uow)
    {
        _ctx = new XpoDataContext(uow);
    }

    public async Task<CompanyDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoCompany>(id, cancellationToken);
            if (xpo == null) return null;

            var domain = XpoCompanyMapper.ToDomain(xpo);

            return XpoCompanyMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<CompanyDto>?> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoCompany>().ToListAsync(cancellationToken);

            if (list == null) return null;
            return list
            .Select(xpo =>
            {
                var domain = XpoCompanyMapper.ToDomain(xpo);

                return XpoCompanyMapper.ToDto(domain);
            })
            .ToList();
        });
    }

    public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoCompany>()
                .FirstOrDefaultAsync(w =>
                    w.Name == dto.Name &&
                    w.Email == dto.Email,
                    cancellationToken);

            if (existing != null)
                throw new Exception("Company already exists");

            // Domain
            var domain = new Company(
                id: Guid.NewGuid(),
                name: dto.Name,
                email: dto.Email
            );

            // XPO
            var xpo = XpoCompanyMapper.ToXpo(domain, uow);

            // Output DTO
            return XpoCompanyMapper.ToDto(domain);
        });
    }

    public async Task<CompanyDto?> UpdateAsync(UpdateCompanyDto dto, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            // 1. Carico l'XPO esistente tramite ID
            var xpo = await uow.GetObjectByKeyAsync<XpoCompany>(dto.Id, cancellationToken);
            if (xpo == null)
                throw new Exception("Company not found");

            // 2. Converto XPO → Domain
            var domain = XpoCompanyMapper.ToDomain(xpo);

            // 3. Aggiorno il Domain con i valori del DTO
            domain.Name = dto.Name;
            domain.Email = dto.Email;

            // 4. Aggiorno l’XPO tramite il mapper
            XpoCompanyMapper.ToXpo(domain, uow);

            // 5. Restituisco il DTO di output
            return XpoCompanyMapper.ToDto(domain);
        });
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoCompany>(id, cancellationToken);
            if (xpo == null)
                throw new Exception("Company not found");

            xpo.Delete();

            return true;
        });
    }
}