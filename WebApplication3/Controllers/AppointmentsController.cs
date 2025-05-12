using Microsoft.AspNetCore.Mvc;
using TestApbdGroupA.Models;
using TestApbdGroupA.Serivces;

namespace TestApbdGroupA.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{   
    private readonly IApoinmentSerivce _apoinmentService;

    public AppointmentsController(IApoinmentSerivce apoinmentSerivce)
    {
        _apoinmentService = apoinmentSerivce;
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApoiinments(int id)
    {
        var visit = await _apoinmentService.GetApoinmentsByIdAsync(id);
        if (visit == null)
            return NotFound();
        return Ok(visit);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddVisit([FromBody] NewApoinmentDto dto)
    {
        try
        {
            await _apoinmentService.AddVisitAsync(dto);
            return CreatedAtAction(nameof(GetApoiinments), new { id = dto.ApoinmentId }, null);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}