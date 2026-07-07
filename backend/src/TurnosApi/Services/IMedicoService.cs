using TurnosApi.Common;
using TurnosApi.DTOs;

namespace TurnosApi.Services;

public interface IMedicoService
{
    Task<ApiResponse<List<MedicoResponse>>> GetAllMedicos();
    Task<ApiResponse<MedicoResponse>> GetMedicoById(int id);
}
