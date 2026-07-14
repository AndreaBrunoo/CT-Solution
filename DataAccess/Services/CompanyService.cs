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
    private readonly IActionLogger _logger;

    public CompanyService(UnitOfWork uow, IActionLogger logger)
    {
        _ctx = new XpoDataContext(uow);
        _logger = logger;
    }

    public async Task<CompanyDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoCompany>(id, ct);
            if (xpo == null) return null;

            var domain = XpoCompanyMapper.ToDomain(xpo);

            return XpoCompanyMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<CompanyDto>?> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoCompany>().ToListAsync(ct);

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

    public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoCompany>()
                .FirstOrDefaultAsync(w =>
                    w.Name == dto.Name &&
                    w.Email == dto.Email,
                    ct);

            if (existing != null)
            {
                await _logger.LogFailureAsync("Create", "Company", null,
                    $"Company '{dto.Name}' already exists", ct);
                throw new Exception("Company already exists");
            }

            var domain = new Company(
                id: Guid.NewGuid(),
                name: dto.Name,
                email: dto.Email
            );

            var xpo = XpoCompanyMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Create", "Company", domain.Id, ct);

            return XpoCompanyMapper.ToDto(domain);
        });
    }

    public async Task<CompanyDto?> UpdateAsync(UpdateCompanyDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoCompany>(dto.Id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("Update", "Company", dto.Id,
                    "Company not found", ct);
                throw new Exception("Company not found");
            }

            var domain = XpoCompanyMapper.ToDomain(xpo);

            domain.Name = dto.Name;
            domain.Email = dto.Email;

            XpoCompanyMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Update", "Company", domain.Id, ct);

            return XpoCompanyMapper.ToDto(domain);
        });
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoCompany>(id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("Delete", "Company", id,
                    "Company not found", ct);
                throw new Exception("Company not found");
            }

            xpo.Delete();

            await _logger.LogSuccessAsync(uow, "Delete", "Company", id, ct);

            return true;
        });
    }
}