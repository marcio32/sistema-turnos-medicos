namespace TurnosApi.Repositories;

public interface IUnitOfWork
{
    ITurnoRepository Turnos { get; }
    IMedicoRepository Medicos { get; }
    IPacienteRepository Pacientes { get; }
    Task<int> SaveChangesAsync();
}
