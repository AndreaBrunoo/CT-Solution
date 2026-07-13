using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;

namespace Sln.Api.Controllers;

[ApiController]
[Route("api/roles")]
public class RolePermissionController : ControllerBase
{
    private readonly IRolePermissionService _service;

    public RolePermissionController(IRolePermissionService service)
    {
        _service = service;
    }

    [HttpPost("{roleId:guid}/permissions/{permissionId:guid}")]
    public async Task<IActionResult> AssignPermission(Guid roleId, Guid permissionId, CancellationToken ct)
    {
        await _service.AssignPermissionAsync(roleId, permissionId, ct);
        return Ok(new { Message = "Permission assigned" });
    }

    [HttpDelete("{roleId:guid}/permissions/{permissionId:guid}")]
    public async Task<IActionResult> RemovePermission(Guid roleId, Guid permissionId, CancellationToken ct)
    {
        await _service.RemovePermissionAsync(roleId, permissionId, ct);
        return Ok(new { Message = "Permission removed" });
    }

    [HttpGet("{roleId:guid}/permissions/check")]
    public async Task<IActionResult> HasPermission(Guid roleId, [FromQuery] string permissionCode, CancellationToken ct)
    {
        var result = await _service.HasPermissionAsync(roleId, permissionCode, ct);
        return Ok(new { HasPermission = result });
    }

    [HttpGet("{roleId:guid}/permissions")]
    public async Task<IActionResult> GetPermissions(Guid roleId, CancellationToken ct)
    {
        var list = await _service.GetPermissionsAsync(roleId, ct);
        return Ok(list);
    }
}