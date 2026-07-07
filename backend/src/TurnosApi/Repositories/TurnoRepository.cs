using Microsoft.EntityFrameworkCore;
using TurnosApi.DTOs;
using TurnosApi.Infrastructure;
using TurnosApi.Models;

namespace TurnosApi.Repositories;

public class TurnoRepository : ITurnoRepository
{
    private readonly AppDbContext _context;

    public TurnoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Turno?> GetByIdAsync(int id)
    {
        return await _context.Turnos
            .Include(t => t.Medico)
            .Include(t => t.Paciente)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Turno>> GetByMedicoYFechaAsync(int medicoId, DateOnly fecha)
    {
        return await _context.Turnos
            .Where(t => t.MedicoId == medicoId && t.Fecha == fecha)
            .Where(t => t.Estado != EstadoTurno.Cancelado)
            .OrderBy(t => t.Hora)
            .ToListAsync();
    }

    public async Task<List<Turno>> GetAllAsync(TurnoFilter filter)
    {
        var query = _context.Turnos
            .Include(t => t.Medico)
            .Include(t => t.Paciente)
            .AsQueryable();

        if (filter.MedicoId.HasValue)
            query = query.Where(t => t.MedicoId == filter.MedicoId.Value);

        if (filter.PacienteId.HasValue)
            query = query.Where(t => t.PacienteId == filter.PacienteId.Value);

        if (filter.Fecha.HasValue)
            query = query.Where(t => t.Fecha == filter.Fecha.Value);

        if (filter.Estado.HasValue)
            query = query.Where(t => t.Estado == filter.Estado.Value);

        return await query
            .OrderByDescending(t => t.Fecha)
            .ThenBy(t => t.Hora)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Turno turno)
    {
        await _context.Turnos.AddAsync(turno);
    }

    public Task UpdateAsync(Turno turno)
    {
        _context.Turnos.Update(turno);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var turno = await _context.Turnos.FindAsync(id);
        if (turno is not null)
        {
            _context.Turnos.Remove(turno);
        }
    }
}
