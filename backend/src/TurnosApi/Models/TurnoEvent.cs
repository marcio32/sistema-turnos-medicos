namespace TurnosApi.Models;

/// <summary>
/// Evento publicado a SQS cuando un turno cambia de estado.
/// Tipos: "turno.creado", "turno.confirmado", "turno.cancelado"
/// </summary>
public class TurnoEvent
{
    public string Tipo { get; set; } = string.Empty;
    public int TurnoId { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly Hora { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> Metadata { get; set; } = new();
}
