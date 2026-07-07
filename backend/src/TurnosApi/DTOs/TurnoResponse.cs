using TurnosApi.Models;

namespace TurnosApi.DTOs;

/// <summary>
/// DTO de respuesta con los datos de un turno.
/// </summary>
public class TurnoResponse
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public string PacienteNombre { get; set; } = string.Empty;
    public int MedicoId { get; set; }
    public string MedicoNombre { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public TimeOnly Hora { get; set; }
    public int Duracion { get; set; }
    public EstadoTurno Estado { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
