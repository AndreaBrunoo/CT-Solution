using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class CategoryService : ICategoryService
{
    private readonly XpoDataContext _ctx;

    public CategoryService(UnitOfWork uow)
    {
        _ctx = new XpoDataContext(uow);
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoCategory>(id, cancellationToken);
            if (xpo == null) return null;

            var domain = XpoCategoryMapper.ToDomain(xpo);

            return XpoCategoryMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<CategoryDto>?> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoCategory>().ToListAsync(cancellationToken);

            if (list == null) return null;
            return list
            .Select(xpo =>
            {
                var domain = XpoCategoryMapper.ToDomain(xpo);

                return XpoCategoryMapper.ToDto(domain);
            })
            .ToList();
        });
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoCategory>()
                .FirstOrDefaultAsync(w =>
                    w.Name == dto.Name,
                    cancellationToken);

            if (existing != null)
                throw new Exception("Category already exists");

            // Domain
            var domain = new Category(
                id: Guid.NewGuid(),
                name: dto.Name
            );

            // XPO
            var xpo = XpoCategoryMapper.ToXpo(domain, uow);

            // Output DTO
            return XpoCategoryMapper.ToDto(domain);
        });
    }

    public async Task<CategoryDto?> UpdateAsync(UpdateCategoryDto dto, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            // 1. Carico l'XPO esistente tramite ID
            var xpo = await uow.GetObjectByKeyAsync<XpoCategory>(dto.Id, cancellationToken);
            if (xpo == null)
                throw new Exception("Category not found");

            // 2. Converto XPO → Domain
            var domain = XpoCategoryMapper.ToDomain(xpo);

            // 3. Aggiorno il Domain con i valori del DTO
            domain.Name = dto.Name;

            // 4. Aggiorno l’XPO tramite il mapper
            XpoCategoryMapper.ToXpo(domain, uow);

            // 5. Restituisco il DTO di output
            return XpoCategoryMapper.ToDto(domain);
        });
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoCategory>(id, cancellationToken);
            if (xpo == null)
                throw new Exception("Category not found");

            xpo.Delete();

            return true;
        });
    }
}