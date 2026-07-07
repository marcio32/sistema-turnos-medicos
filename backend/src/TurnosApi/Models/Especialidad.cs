namespace TurnosApi.Models;

public class Especialidad
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    // Navigation properties
    public List<Medico> Medicos { get; set; } = new();
}
