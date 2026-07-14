using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;

namespace Sln.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var project = await _projectService.GetByIdAsync(id, ct);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var project = await _projectService.GetAllAsync(ct);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateProjectDto dto, CancellationToken ct)
    {
        await _projectService.CreateAsync(dto, ct);
        return Ok(new { message = "Creation successful" });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateProjectDto dto, CancellationToken ct)
    {
        await _projectService.UpdateAsync(dto, ct);
        return Ok(new { message = "Update successful" });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _projectService.DeleteAsync(id, ct);
        return Ok(new { message = "Delete successful" });
    }
}