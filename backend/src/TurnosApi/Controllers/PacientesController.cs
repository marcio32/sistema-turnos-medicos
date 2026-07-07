using Microsoft.AspNetCore.Mvc;
using TurnosApi.Common;
using TurnosApi.Models;
using TurnosApi.Repositories;

namespace TurnosApi.Controllers;

[Route("api/pacientes")]
[ApiController]
public class PacientesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public PacientesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Lista todos los pacientes.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Paciente>>>> GetAll()
    {
        var pacientes = await _unitOfWork.Pacientes.GetAllAsync();
        var response = ApiResponse<List<Paciente>>.Success(pacientes);
        return Ok(response);
    }

    /// <summary>
    /// Obtiene un paciente por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Paciente>>> GetById(int id)
    {
        var paciente = await _unitOfWork.Pacientes.GetByIdAsync(id);

        if (paciente == null)
        {
            var notFound = ApiResponse<Paciente>.Fail(
                "Paciente no encontrado", "id", "NOT_FOUND", 404);
            return NotFound(notFound);
        }

        var response = ApiResponse<Paciente>.Success(paciente);
        return Ok(response);
    }
}
