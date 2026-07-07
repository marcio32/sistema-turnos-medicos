namespace TurnosApi.Models;

public class Paciente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;

    // Navigation properties
    public List<Turno> Turnos { get; set; } = new();
}
