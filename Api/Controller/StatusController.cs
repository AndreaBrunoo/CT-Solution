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
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var status = await _statusService.GetByIdAsync(id, cancellationToken);
        return status == null ? NotFound() : Ok(status);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var status = await _statusService.GetAllAsync(cancellationToken);
        return status == null ? NotFound() : Ok(status);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateStatusDto dto, CancellationToken cancellationToken)
    {
        await _statusService.CreateAsync(dto, cancellationToken);
        return Ok(new { message = "Creation successful" });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateStatusDto dto, CancellationToken cancellationToken)
    {
        await _statusService.UpdateAsync(dto, cancellationToken);
        return Ok(new { message = "Update successful" });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _statusService.DeleteAsync(id, cancellationToken);
        return Ok(new { message = "Delete successful" });
    }
}