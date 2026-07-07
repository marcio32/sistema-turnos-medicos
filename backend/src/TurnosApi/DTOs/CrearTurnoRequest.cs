using System.ComponentModel.DataAnnotations;

namespace TurnosApi.DTOs;

/// <summary>
/// DTO para la creación de un nuevo turno médico.
/// </summary>
public class CrearTurnoRequest
{
    [Required(ErrorMessage = "El paciente es obligatorio")]
    public int PacienteId { get; set; }

    [Required(ErrorMessage = "El médico es obligatorio")]
    public int MedicoId { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateOnly Fecha { get; set; }

    [Required(ErrorMessage = "La hora es obligatoria")]
    public TimeOnly Hora { get; set; }

    [Required(ErrorMessage = "La duración es obligatoria")]
    [Range(30, 60, ErrorMessage = "La duración debe ser 30 o 60 minutos")]
    public int Duracion { get; set; }

    [Required(ErrorMessage = "El motivo es obligatorio")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "El motivo debe tener entre 3 y 500 caracteres")]
    public string Motivo { get; set; } = string.Empty;
}
