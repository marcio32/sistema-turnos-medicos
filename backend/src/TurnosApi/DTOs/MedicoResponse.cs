namespace TurnosApi.DTOs;

/// <summary>
/// DTO de respuesta con los datos de un médico.
/// </summary>
public class MedicoResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
}
