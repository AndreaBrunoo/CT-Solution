using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Sln.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var category = await _categoryService.GetByIdAsync(id, ct);
        return category == null ? NotFound() : Ok(category);
    }

    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var category = await _categoryService.GetAllAsync(ct);
        return category == null ? NotFound() : Ok(category);
    }

    [Authorize(Roles = "ProjectManager, Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateCategoryDto dto, CancellationToken ct)
    {
        await _categoryService.CreateAsync(dto, ct);
        return Ok(new { message = "Creation successful" });
    }

    [Authorize(Roles = "ProjectManager, Admin")]
    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateCategoryDto dto, CancellationToken ct)
    {
        await _categoryService.UpdateAsync(dto, ct);
        return Ok(new { message = "Update successful" });
    }

    [Authorize(Roles = "ProjectManager, Admin")]
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _categoryService.DeleteAsync(id, ct);
        return Ok(new { message = "Delete successful" });
    }

    [Authorize(Roles = "ProjectManager, Admin")]
    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        await _categoryService.RestoreAsync(id, ct);
        return Ok(new { message = "Restore successful" });
    }
}