namespace TurnosApi.Models;

public class Medico
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public int EspecialidadId { get; set; }

    // Navigation properties
    public Especialidad Especialidad { get; set; } = null!;
    public List<Turno> Turnos { get; set; } = new();
}
