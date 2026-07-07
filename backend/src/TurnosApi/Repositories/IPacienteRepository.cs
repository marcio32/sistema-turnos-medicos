using TurnosApi.Models;

namespace TurnosApi.Repositories;

public interface IPacienteRepository
{
    Task<Paciente?> GetByIdAsync(int id);
    Task<Paciente?> GetByDniAsync(string dni);
    Task<List<Paciente>> GetAllAsync();
    Task AddAsync(Paciente paciente);
}
