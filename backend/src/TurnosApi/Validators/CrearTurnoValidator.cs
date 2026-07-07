using FluentValidation;
using TurnosApi.DTOs;

namespace TurnosApi.Validators;

/// <summary>
/// Validator para la creación de un nuevo turno médico.
/// Aplica reglas de negocio: campos requeridos, fecha futura, hora laboral, duración válida.
/// </summary>
public class CrearTurnoValidator : AbstractValidator<CrearTurnoRequest>
{
    public CrearTurnoValidator()
    {
        RuleFor(x => x.PacienteId)
            .NotEmpty().WithMessage("El paciente es obligatorio")
            .GreaterThan(0).WithMessage("El ID del paciente debe ser mayor a 0");

        RuleFor(x => x.MedicoId)
            .NotEmpty().WithMessage("El médico es obligatorio")
            .GreaterThan(0).WithMessage("El ID del médico debe ser mayor a 0");

        RuleFor(x => x.Fecha)
            .NotEmpty().WithMessage("La fecha es obligatoria")
            .Must(fecha => fecha > DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("La fecha debe ser futura");

        RuleFor(x => x.Hora)
            .NotEmpty().WithMessage("La hora es obligatoria")
            .Must(hora => hora >= new TimeOnly(8, 0) && hora <= new TimeOnly(19, 30))
            .WithMessage("La hora debe estar entre las 08:00 y las 19:30");

        RuleFor(x => x.Duracion)
            .Must(duracion => duracion == 30 || duracion == 60)
            .WithMessage("La duración debe ser 30 o 60 minutos");

        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("El motivo es obligatorio")
            .MinimumLength(3).WithMessage("El motivo debe tener al menos 3 caracteres")
            .MaximumLength(500).WithMessage("El motivo no puede superar los 500 caracteres");
    }
}
