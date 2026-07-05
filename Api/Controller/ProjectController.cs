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
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetByIdAsync(id, cancellationToken);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var project = await _projectService.GetAllAsync(cancellationToken);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateProjectDto dto, CancellationToken cancellationToken)
    {
        await _projectService.CreateAsync(dto, cancellationToken);
        return Ok(new { message = "Creation successful" });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateProjectDto dto, CancellationToken cancellationToken)
    {
        await _projectService.UpdateAsync(dto, cancellationToken);
        return Ok(new { message = "Update successful" });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _projectService.DeleteAsync(id, cancellationToken);
        return Ok(new { message = "Delete successful" });
    }
}