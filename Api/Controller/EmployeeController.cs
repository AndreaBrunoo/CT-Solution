using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace Sln.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var employee = await _employeeService.GetByIdAsync(id, ct);
        return employee == null ? NotFound() : Ok(employee);
    }

    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var employee = await _employeeService.GetAllAsync(ct);
        return employee == null ? NotFound() : Ok(employee);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var employee = await _employeeService.GetByUserIdAsync(userId, ct);
        return employee == null ? NotFound() : Ok(employee);
    }

    [Authorize]
    [HttpGet("by-project/{projectId:guid}")]
    public async Task<IActionResult> GetByProject(Guid projectId, CancellationToken ct)
    {
        var employees = await _employeeService.GetByProjectIdAsync(projectId, ct);
        return employees == null ? NotFound(new { Message = "Project not found" }) : Ok(employees);
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateEmployeeDto dto, CancellationToken ct)
    {
        await _employeeService.UpdateAsync(dto, ct);
        return Ok(new { message = "Update successful" });
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _employeeService.DeleteAsync(id, ct);
        return Ok(new { message = "Delete successful" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        await _employeeService.RestoreAsync(id, ct);
        return Ok(new { message = "Restore successful" });
    }
}