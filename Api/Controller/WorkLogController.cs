using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Sln.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkLogController : ControllerBase
{
    private readonly IWorkLogService _workLogService;

    public WorkLogController(IWorkLogService workLogService)
    {
        _workLogService = workLogService;
    }

    [Authorize(Roles = "Admin, ProjectManager")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var workLog = await _workLogService.GetByIdAsync(id, ct);
        return workLog == null ? NotFound() : Ok(workLog);
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        var result = await _workLogService.GetMineAsync(Guid.Parse(userId), HttpContext.RequestAborted);

        return Ok(result);
    }

    [Authorize(Roles = "Admin, ProjectManager")]
    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var workLog = await _workLogService.GetAllAsync(ct);
        return workLog == null ? NotFound() : Ok(workLog);
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateWorkLogDto dto, CancellationToken ct)
    {
        await _workLogService.CreateAsync(dto, ct);
        return Ok(new { message = "Creation successful" });
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateWorkLogDto dto, CancellationToken ct)
    {
        await _workLogService.UpdateAsync(dto, ct);
        return Ok(new { message = "Update successful" });
    }

    [Authorize(Roles = "ProjectManager, Admin")]
    [HttpPost("{id}/change-status")]
    public async Task<IActionResult> ChangeStatus(Guid id, Guid newStatusId)
    {
        var result = await _workLogService.ChangeStatusAsync(id, newStatusId, HttpContext.RequestAborted);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _workLogService.DeleteAsync(id, ct);
        return Ok(new { message = "Delete successful" });
    }

    [Authorize(Roles = "ProjectManager, Admin")]
    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        await _workLogService.RestoreAsync(id, ct);
        return Ok(new { message = "Restore successful" });
    }
}