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
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var company = await _companyService.GetByIdAsync(id, cancellationToken);
        return company == null ? NotFound() : Ok(company);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var company = await _companyService.GetAllAsync(cancellationToken);
        return company == null ? NotFound() : Ok(company);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateCompanyDto dto, CancellationToken cancellationToken)
    {
        await _companyService.CreateAsync(dto, cancellationToken);
        return Ok(new { message = "Creation successful" });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateCompanyDto dto, CancellationToken cancellationToken)
    {
        await _companyService.UpdateAsync(dto, cancellationToken);
        return Ok(new { message = "Update successful" });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _companyService.DeleteAsync(id, cancellationToken);
        return Ok(new { message = "Delete successful" });
    }
}