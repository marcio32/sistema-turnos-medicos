using TurnosApi.Common;
using TurnosApi.DTOs;
using TurnosApi.Exceptions;
using TurnosApi.Repositories;

namespace TurnosApi.Services;

public class MedicoService : IMedicoService
{
    private readonly IUnitOfWork _unitOfWork;

    public MedicoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<MedicoResponse>>> GetAllMedicos()
    {
        var medicos = await _unitOfWork.Medicos.GetAllAsync();

        var response = medicos.Select(m => new MedicoResponse
        {
            Id = m.Id,
            Nombre = m.Nombre,
            Matricula = m.Matricula,
            Especialidad = m.Especialidad?.Nombre ?? string.Empty
        }).ToList();

        return ApiResponse<List<MedicoResponse>>.Success(response, "Médicos obtenidos exitosamente");
    }

    public async Task<ApiResponse<MedicoResponse>> GetMedicoById(int id)
    {
        var medico = await _unitOfWork.Medicos.GetByIdAsync(id);
        if (medico is null)
        {
            throw new NotFoundException("Médico", id);
        }

        var response = new MedicoResponse
        {
            Id = medico.Id,
            Nombre = medico.Nombre,
            Matricula = medico.Matricula,
            Especialidad = medico.Especialidad?.Nombre ?? string.Empty
        };

        return ApiResponse<MedicoResponse>.Success(response, "Médico obtenido exitosamente");
    }
}
