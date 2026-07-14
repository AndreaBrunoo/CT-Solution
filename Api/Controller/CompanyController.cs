using Microsoft.AspNetCore.Mvc;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;

namespace Sln.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var company = await _companyService.GetByIdAsync(id, ct);
        return company == null ? NotFound() : Ok(company);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var company = await _companyService.GetAllAsync(ct);
        return company == null ? NotFound() : Ok(company);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateCompanyDto dto, CancellationToken ct)
    {
        await _companyService.CreateAsync(dto, ct);
        return Ok(new { message = "Creation successful" });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateCompanyDto dto, CancellationToken ct)
    {
        await _companyService.UpdateAsync(dto, ct);
        return Ok(new { message = "Update successful" });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _companyService.DeleteAsync(id, ct);
        return Ok(new { message = "Delete successful" });
    }
}