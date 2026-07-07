using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TurnosApi.Common;
using TurnosApi.DTOs;
using TurnosApi.Infrastructure;
using TurnosApi.Models;
using TurnosApi.Services;

namespace TurnosApi.Controllers;

[Route("api/turnos")]
[ApiController]
public class TurnosController : ControllerBase
{
    private readonly ITurnoService _turnoService;
    private readonly ISqsPublisher _sqsPublisher;

    public TurnosController(ITurnoService turnoService, ISqsPublisher sqsPublisher)
    {
        _turnoService = turnoService;
        _sqsPublisher = sqsPublisher;
    }

    /// <summary>
    /// Lista turnos con paginación y filtros opcionales.
    /// </summary>
    [HttpGet]
    [OutputCache(PolicyName = "TurnosPolicy")]
    public async Task<ActionResult<ApiResponse<List<TurnoResponse>>>> GetAll([FromQuery] TurnoFilter filter)
    {
        var response = await _turnoService.ListarTurnos(filter);
        return StatusCode(response.Status.Code, response);
    }

    /// <summary>
    /// Obtiene un turno por su ID.
    /// </summary>
    [HttpGet("{id}")]
    [OutputCache(PolicyName = "TurnosPolicy")]
    public async Task<ActionResult<ApiResponse<TurnoResponse>>> GetById(int id)
    {
        var filter = new TurnoFilter { Page = 1, PageSize = int.MaxValue };
        var listResponse = await _turnoService.ListarTurnos(filter);

        var turno = listResponse.Data?.FirstOrDefault(t => t.Id == id);

        if (turno == null)
        {
            var notFound = ApiResponse<TurnoResponse>.Fail(
                "Turno no encontrado", "id", "NOT_FOUND", 404);
            return NotFound(notFound);
        }

        var response = ApiResponse<TurnoResponse>.Success(turno);
        return Ok(response);
    }

    /// <summary>
    /// Crea un nuevo turno.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TurnoResponse>>> Create([FromBody] CrearTurnoRequest request)
    {
        var response = await _turnoService.CrearTurno(request);
        if (response.Status.Code >= 400)
        {
            return StatusCode(response.Status.Code, response);
        }
        return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
    }

    /// <summary>
    /// Actualiza un turno existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TurnoResponse>>> Update(int id, [FromBody] ActualizarTurnoRequest request)
    {
        // Validate the turno exists first
        var filter = new TurnoFilter { Page = 1, PageSize = int.MaxValue };
        var listResponse = await _turnoService.ListarTurnos(filter);
        var existing = listResponse.Data?.FirstOrDefault(t => t.Id == id);

        if (existing == null)
        {
            var notFound = ApiResponse<TurnoResponse>.Fail(
                "Turno no encontrado", "id", "NOT_FOUND", 404);
            return NotFound(notFound);
        }

        // Map update request to create request for the service (re-create pattern)
        var crearRequest = new CrearTurnoRequest
        {
            PacienteId = request.PacienteId,
            MedicoId = request.MedicoId,
            Fecha = request.Fecha,
            Hora = request.Hora,
            Duracion = request.Duracion,
            Motivo = request.Motivo
        };

        // Cancel old and create new
        await _turnoService.CancelarTurno(id);
        var response = await _turnoService.CrearTurno(crearRequest);

        if (response.Status.Code >= 400)
        {
            return StatusCode(response.Status.Code, response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Cancela (elimina lógicamente) un turno.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<TurnoResponse>>> Delete(int id)
    {
        var response = await _turnoService.CancelarTurno(id);
        if (response.Status.Code >= 400)
        {
            return StatusCode(response.Status.Code, response);
        }
        return Ok(response);
    }

    /// <summary>
    /// Confirma un turno pendiente.
    /// </summary>
    [HttpPost("{id}/confirmar")]
    public async Task<ActionResult<ApiResponse<TurnoResponse>>> Confirmar(int id)
    {
        var response = await _turnoService.ConfirmarTurno(id);
        if (response.Status.Code >= 400)
        {
            return StatusCode(response.Status.Code, response);
        }
        return Ok(response);
    }

    /// <summary>
    /// Envía un recordatorio del turno via cola SQS.
    /// Este endpoint publica un evento de tipo "turno.recordatorio" a la cola
    /// para que el Worker Service lo procese y simule el envío de notificación.
    /// </summary>
    [HttpPost("{id}/recordatorio")]
    public async Task<ActionResult<ApiResponse<object>>> EnviarRecordatorio(int id, [FromBody] RecordatorioRequest? request)
    {
        // Verificar que el turno existe
        var filter = new TurnoFilter { Page = 1, PageSize = int.MaxValue };
        var listResponse = await _turnoService.ListarTurnos(filter);
        var turno = listResponse.Data?.FirstOrDefault(t => t.Id == id);

        if (turno == null)
        {
            var notFound = ApiResponse<object>.Fail(
                "Turno no encontrado", "id", "NOT_FOUND", 404);
            return NotFound(notFound);
        }

        // Publicar evento de recordatorio a SQS
        await _sqsPublisher.PublishAsync(new TurnoEvent
        {
            Tipo = "turno.recordatorio",
            TurnoId = id,
            PacienteId = turno.PacienteId,
            MedicoId = turno.MedicoId,
            Fecha = turno.Fecha,
            Hora = turno.Hora,
            Metadata = new Dictionary<string, string>
            {
                ["mensaje"] = request?.Mensaje ?? "Recordatorio: tiene un turno programado."
            }
        });

        var result = new
        {
            turnoId = id,
            mensaje = request?.Mensaje ?? "Recordatorio: tiene un turno programado.",
            estado = "enviado"
        };

        return Ok(ApiResponse<object>.Success(result, "Recordatorio enviado exitosamente"));
    }
}
