using TurnosApi.Common;
using TurnosApi.DTOs;

namespace TurnosApi.Services;

public interface ITurnoService
{
    Task<ApiResponse<TurnoResponse>> CrearTurno(CrearTurnoRequest request);
    Task<ApiResponse<TurnoResponse>> ConfirmarTurno(int turnoId);
    Task<ApiResponse<TurnoResponse>> CancelarTurno(int turnoId);
    Task<ApiResponse<List<TurnoResponse>>> ListarTurnos(TurnoFilter filter);
    Task<ApiResponse<DisponibilidadResponse>> ConsultarDisponibilidad(int medicoId, DateOnly fecha);
}
