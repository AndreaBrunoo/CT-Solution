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
    private readonly IActionLogger _logger;

    public CategoryService(UnitOfWork uow, IActionLogger logger)
    {
        _ctx = new XpoDataContext(uow);
        _logger = logger;
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoCategory>(id, ct);
            if (xpo == null || xpo.IsDeleted) return null;

            var domain = XpoCategoryMapper.ToDomain(xpo);

            return XpoCategoryMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<CategoryDto>?> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoCategory>()
                .Where(x => x.DeletedAt == null)
                .ToListAsync(ct);

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

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoCategory>()
                .FirstOrDefaultAsync(w =>
                    w.DeletedAt == null && w.Name == dto.Name,
                    ct);

            if (existing != null)
            {
                await _logger.LogFailureAsync("Create", "Category", null,
                    $"Category with name '{dto.Name}' already exists", ct);
                throw new Exception("Category already exists");
            }

            // Domain
            var domain = new Category(
                id: Guid.NewGuid(),
                name: dto.Name
            );

            var xpo = XpoCategoryMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Create", "Category", domain.Id, ct);

            return XpoCategoryMapper.ToDto(domain);
        });
    }

    public async Task<CategoryDto?> UpdateAsync(UpdateCategoryDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoCategory>(dto.Id, ct);
            if (xpo == null || xpo.IsDeleted)
            {
                await _logger.LogFailureAsync("Update", "Category", dto.Id,
                    "Category not found", ct);
                throw new Exception("Category not found");
            }

            var domain = XpoCategoryMapper.ToDomain(xpo);

            domain.Name = dto.Name;

            XpoCategoryMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Update", "Category", domain.Id, ct);

            return XpoCategoryMapper.ToDto(domain);
        });
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoCategory>(id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("SoftDelete", "Category", id,
                    "Category not found", ct);
                throw new Exception("Category not found");
            }

            if (!xpo.IsDeleted)
            {
                xpo.IsDeleted = true;
                xpo.DeletedAt = DateTime.UtcNow;
            }

            await _logger.LogSuccessAsync(uow, "SoftDelete", "Category", id, ct);

            return true;
        });
    }

    public async Task RestoreAsync(Guid id, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoCategory>(id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("Restore", "Category", id,
                    "Category not found", ct);
                throw new Exception("Category not found");
            }

            xpo.IsDeleted = false;
            xpo.DeletedAt = null;

            await _logger.LogSuccessAsync(uow, "Restore", "Category", id, ct);

            return true;
        });
    }
}
