using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;

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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var employee = await _employeeService.GetByIdAsync(id, ct);
        return employee == null ? NotFound() : Ok(employee);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var employee = await _employeeService.GetAllAsync(ct);
        return employee == null ? NotFound() : Ok(employee);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateEmployeeDto dto, CancellationToken ct)
    {
        await _employeeService.CreateAsync(dto, ct);
        return Ok(new { message = "Creation successful" });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateEmployeeDto dto, CancellationToken ct)
    {
        await _employeeService.UpdateAsync(dto, ct);
        return Ok(new { message = "Update successful" });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _employeeService.DeleteAsync(id, ct);
        return Ok(new { message = "Delete successful" });
    }
}