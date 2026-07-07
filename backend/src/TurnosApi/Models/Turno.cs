namespace TurnosApi.Models;

public class Turno
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly Hora { get; set; }
    public int Duracion { get; set; } // 30 o 60 minutos
    public EstadoTurno Estado { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Paciente Paciente { get; set; } = null!;
    public Medico Medico { get; set; } = null!;
}
