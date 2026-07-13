using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;

namespace Sln.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        await _userService.RegisterAsync(dto.Email, dto.Password);
        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _userService.LoginAsync(dto.Email, dto.Password);
        return Ok(new AuthResponseDto { Token = token });
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var user = await _userService.GetAllAsync(cancellationToken);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByEmailAsync(email, cancellationToken);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost("{userId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> AssignRole(Guid userId, Guid roleId, CancellationToken ct)
    {
        await _userService.AssignRoleAsync(userId, roleId, ct);
        return Ok(new { Message = "Role assigned" });
    }

    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId, CancellationToken ct)
    {
        await _userService.RemoveRoleAsync(userId, roleId, ct);
        return Ok(new { Message = "Role removed" });
    }

    [HttpGet("{userId:guid}/roles/check")]
    public async Task<IActionResult> HasRole(Guid userId, [FromQuery] string roleName, CancellationToken ct)
    {
        var result = await _userService.HasRoleAsync(userId, roleName, ct);
        return Ok(new { HasRole = result });
    }

    [HttpGet("{userId:guid}/permissions/check")]
    public async Task<IActionResult> HasPermission(Guid userId, [FromQuery] string permissionCode, CancellationToken ct)
    {
        var result = await _userService.HasPermissionAsync(userId, permissionCode, ct);
        return Ok(new { HasPermission = result });
    }
}