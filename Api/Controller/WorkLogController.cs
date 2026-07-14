using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;

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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var workLog = await _workLogService.GetByIdAsync(id, ct);
        return workLog == null ? NotFound() : Ok(workLog);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var workLog = await _workLogService.GetAllAsync(ct);
        return workLog == null ? NotFound() : Ok(workLog);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateWorkLogDto dto, CancellationToken ct)
    {
        await _workLogService.CreateAsync(dto, ct);
        return Ok(new { message = "Creation successful" });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateWorkLogDto dto, CancellationToken ct)
    {
        await _workLogService.UpdateAsync(dto, ct);
        return Ok(new { message = "Update successful" });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _workLogService.DeleteAsync(id, ct);
        return Ok(new { message = "Delete successful" });
    }
}