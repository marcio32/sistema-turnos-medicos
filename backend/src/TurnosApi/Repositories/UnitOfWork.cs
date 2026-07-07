using TurnosApi.Infrastructure;

namespace TurnosApi.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private ITurnoRepository? _turnos;
    private IMedicoRepository? _medicos;
    private IPacienteRepository? _pacientes;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ITurnoRepository Turnos =>
        _turnos ??= new TurnoRepository(_context);

    public IMedicoRepository Medicos =>
        _medicos ??= new MedicoRepository(_context);

    public IPacienteRepository Pacientes =>
        _pacientes ??= new PacienteRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
