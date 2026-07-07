using Microsoft.EntityFrameworkCore;
using TurnosApi.Infrastructure;
using TurnosApi.Models;

namespace TurnosApi.Repositories;

public class MedicoRepository : IMedicoRepository
{
    private readonly AppDbContext _context;

    public MedicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Medico?> GetByIdAsync(int id)
    {
        return await _context.Medicos
            .Include(m => m.Especialidad)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Medico>> GetAllAsync()
    {
        return await _context.Medicos
            .Include(m => m.Especialidad)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<List<Medico>> GetByEspecialidadAsync(int especialidadId)
    {
        return await _context.Medicos
            .Include(m => m.Especialidad)
            .Where(m => m.EspecialidadId == especialidadId)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }
}
