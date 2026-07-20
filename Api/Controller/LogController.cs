using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class LogController : ControllerBase
{
    private readonly IActionLogger _service;

    public LogController(IActionLogger service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var logs = await _service.GetAllAsync(ct);
        return Ok(logs);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var log = await _service.GetByIdAsync(id, ct);
        return log is null ? NotFound() : Ok(log);
    }
}