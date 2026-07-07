using TurnosApi.Models;

namespace TurnosApi.DTOs;

public class TurnoFilter
{
    public int? MedicoId { get; set; }
    public int? PacienteId { get; set; }
    public DateOnly? Fecha { get; set; }
    public EstadoTurno? Estado { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
