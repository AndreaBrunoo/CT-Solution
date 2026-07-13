using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;

namespace Sln.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(string code, CancellationToken ct)
    {
        await _permissionService.CreatePermissionAsync(code, ct);
        return Ok(new { message = "Creation successful" });
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var permission = await _permissionService.GetAllAsync(ct);
        return permission == null ? NotFound() : Ok(permission);
    }

    [HttpDelete()]
    public async Task<IActionResult> DeleteRole(Guid permissionId, CancellationToken ct)
    {
        await _permissionService.DeletePermissionAsync(permissionId, ct);
        return Ok(new { message = "Delete successful" });
    }
}