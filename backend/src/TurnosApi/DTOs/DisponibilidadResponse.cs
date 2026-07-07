namespace TurnosApi.DTOs;

/// <summary>
/// DTO de respuesta con la disponibilidad de un médico en una fecha.
/// </summary>
public class DisponibilidadResponse
{
    public int MedicoId { get; set; }
    public string MedicoNombre { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public List<TimeOnly> SlotsDisponibles { get; set; } = new();
}
