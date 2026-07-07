using Microsoft.EntityFrameworkCore;
using TurnosApi.Infrastructure;
using TurnosApi.Models;

namespace TurnosApi.Repositories;

public class PacienteRepository : IPacienteRepository
{
    private readonly AppDbContext _context;

    public PacienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Paciente?> GetByIdAsync(int id)
    {
        return await _context.Pacientes
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Paciente?> GetByDniAsync(string dni)
    {
        return await _context.Pacientes
            .FirstOrDefaultAsync(p => p.DNI == dni);
    }

    public async Task<List<Paciente>> GetAllAsync()
    {
        return await _context.Pacientes
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task AddAsync(Paciente paciente)
    {
        await _context.Pacientes.AddAsync(paciente);
    }
}
