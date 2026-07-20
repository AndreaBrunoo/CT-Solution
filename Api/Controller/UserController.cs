using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Sln.Domain.Interfaces;
using System.Security.Claims;
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

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto, CancellationToken ct)
    {
        await _userService.RegisterAsync(dto, ct);
        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto, CancellationToken ct)
    {
        var token = await _userService.LoginAsync(dto, ct);
        return Ok(new AuthResponseDto { Token = token });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var user = await _userService.GetAllAsync(ct);
        return user == null ? NotFound() : Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var user = await _userService.GetByIdAsync(id, ct);
        return user == null ? NotFound() : Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email, CancellationToken ct)
    {
        var user = await _userService.GetByEmailAsync(email, ct);
        return user == null ? NotFound() : Ok(user);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("{userId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> AssignRole(Guid userId, Guid roleId, CancellationToken ct)
    {
        await _userService.AssignRoleAsync(userId, roleId, ct);
        return Ok(new { Message = "Role assigned" });
    }

    [Authorize]
    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

        return Ok(new CurrentUserDto
        {
            Id = Guid.Parse(userId),
            Email = email,
            Roles = roles
        });
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId, CancellationToken ct)
    {
        await _userService.RemoveRoleAsync(userId, roleId, ct);
        return Ok(new { Message = "Role removed" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{userId:guid}/roles/check")]
    public async Task<IActionResult> HasRole(Guid userId, [FromQuery] string roleName, CancellationToken ct)
    {
        var result = await _userService.HasRoleAsync(userId, roleName, ct);
        return Ok(new { HasRole = result });
    }

    [Authorize]
    [HttpPut("{userId:guid}/password")]
    public async Task<IActionResult> UpdatePassword(Guid userId, UpdatePasswordDto dto, CancellationToken ct)
    {
        var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && callerId != userId.ToString())
            return Forbid();

        await _userService.UpdatePasswordAsync(userId, dto, ct);
        return Ok(new { Message = "Password updated" });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> Delete(Guid userId, CancellationToken ct)
    {
        await _userService.DeleteAsync(userId, ct);
        return Ok(new { Message = "User deleted" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{userId:guid}/restore")]
    public async Task<IActionResult> Restore(Guid userId, CancellationToken ct)
    {
        await _userService.RestoreAsync(userId, ct);
        return Ok(new { Message = "User restored" });
    }
}