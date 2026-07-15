using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;

namespace Sln.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateRoleDto dto, CancellationToken ct)
    {
        await _roleService.CreateRoleAsync(dto, ct);
        return Ok(new { message = "Creation successful" });
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var role = await _roleService.GetAllAsync(ct);
        return role == null ? NotFound() : Ok(role);
    }

    [HttpDelete()]
    public async Task<IActionResult> DeleteRole(Guid roleId, CancellationToken ct)
    {
        await _roleService.DeleteRoleAsync(roleId, ct);
        return Ok(new { message = "Delete successful" });
    }
}