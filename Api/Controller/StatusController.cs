using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;

namespace Sln.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    private readonly IStatusService _statusService;

    public StatusController(IStatusService statusService)
    {
        _statusService = statusService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var status = await _statusService.GetByIdAsync(id, ct);
        return status == null ? NotFound() : Ok(status);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var status = await _statusService.GetAllAsync(ct);
        return status == null ? NotFound() : Ok(status);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateStatusDto dto, CancellationToken ct)
    {
        await _statusService.CreateAsync(dto, ct);
        return Ok(new { message = "Creation successful" });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateStatusDto dto, CancellationToken ct)
    {
        await _statusService.UpdateAsync(dto, ct);
        return Ok(new { message = "Update successful" });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _statusService.DeleteAsync(id, ct);
        return Ok(new { message = "Delete successful" });
    }
}