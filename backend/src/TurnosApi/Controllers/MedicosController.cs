using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TurnosApi.Common;
using TurnosApi.DTOs;
using TurnosApi.Services;

namespace TurnosApi.Controllers;

[Route("api/medicos")]
[ApiController]
public class MedicosController : ControllerBase
{
    private readonly IMedicoService _medicoService;
    private readonly ITurnoService _turnoService;

    public MedicosController(IMedicoService medicoService, ITurnoService turnoService)
    {
        _medicoService = medicoService;
        _turnoService = turnoService;
    }

    /// <summary>
    /// Lista todos los médicos.
    /// </summary>
    [HttpGet]
    [OutputCache(PolicyName = "MedicosPolicy")]
    public async Task<ActionResult<ApiResponse<List<MedicoResponse>>>> GetAll()
    {
        var response = await _medicoService.GetAllMedicos();
        return StatusCode(response.Status.Code, response);
    }

    /// <summary>
    /// Obtiene un médico por su ID.
    /// </summary>
    [HttpGet("{id}")]
    [OutputCache(PolicyName = "MedicosPolicy")]
    public async Task<ActionResult<ApiResponse<MedicoResponse>>> GetById(int id)
    {
        var response = await _medicoService.GetMedicoById(id);
        if (response.Status.Code >= 400)
        {
            return StatusCode(response.Status.Code, response);
        }
        return Ok(response);
    }

    /// <summary>
    /// Consulta la disponibilidad de un médico en una fecha determinada.
    /// </summary>
    [HttpGet("{id}/disponibilidad")]
    [OutputCache(PolicyName = "DisponibilidadPolicy")]
    public async Task<ActionResult<ApiResponse<DisponibilidadResponse>>> GetDisponibilidad(
        int id, [FromQuery] DateOnly fecha)
    {
        var response = await _turnoService.ConsultarDisponibilidad(id, fecha);
        if (response.Status.Code >= 400)
        {
            return StatusCode(response.Status.Code, response);
        }
        return Ok(response);
    }
}
