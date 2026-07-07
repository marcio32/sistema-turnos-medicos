using TurnosApi.Models;

namespace TurnosApi.Exceptions;

/// <summary>
/// Excepción lanzada cuando se intenta una transición de estado inválida en un turno.
/// </summary>
public class TransicionInvalidaException : Exception
{
    public EstadoTurno EstadoActual { get; }
    public EstadoTurno EstadoDeseado { get; }

    public TransicionInvalidaException()
        : base("La transición de estado solicitada no es válida.") { }

    public TransicionInvalidaException(string message)
        : base(message) { }

    public TransicionInvalidaException(EstadoTurno estadoActual, EstadoTurno estadoDeseado)
        : base($"No se puede transicionar de '{estadoActual}' a '{estadoDeseado}'.")
    {
        EstadoActual = estadoActual;
        EstadoDeseado = estadoDeseado;
    }

    public TransicionInvalidaException(string message, Exception innerException)
        : base(message, innerException) { }
}
