using TurnosApi.Models;

namespace TurnosApi.Repositories;

public interface IMedicoRepository
{
    Task<Medico?> GetByIdAsync(int id);
    Task<List<Medico>> GetAllAsync();
    Task<List<Medico>> GetByEspecialidadAsync(int especialidadId);
}
