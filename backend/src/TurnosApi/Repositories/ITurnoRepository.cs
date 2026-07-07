using TurnosApi.DTOs;
using TurnosApi.Models;

namespace TurnosApi.Repositories;

public interface ITurnoRepository
{
    Task<Turno?> GetByIdAsync(int id);
    Task<List<Turno>> GetByMedicoYFechaAsync(int medicoId, DateOnly fecha);
    Task<List<Turno>> GetAllAsync(TurnoFilter filter);
    Task AddAsync(Turno turno);
    Task UpdateAsync(Turno turno);
    Task DeleteAsync(int id);
}
