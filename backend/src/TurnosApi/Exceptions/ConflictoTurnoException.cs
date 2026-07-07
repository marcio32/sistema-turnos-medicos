namespace TurnosApi.Exceptions;

/// <summary>
/// Excepción lanzada cuando se detecta un conflicto de turno (solapamiento de horarios).
/// </summary>
public class ConflictoTurnoException : Exception
{
    public ConflictoTurnoException()
        : base("Existe un conflicto con otro turno en el mismo horario.") { }

    public ConflictoTurnoException(string message)
        : base(message) { }

    public ConflictoTurnoException(string message, Exception innerException)
        : base(message, innerException) { }
}
