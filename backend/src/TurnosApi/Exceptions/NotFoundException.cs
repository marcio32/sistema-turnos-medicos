namespace TurnosApi.Exceptions;

/// <summary>
/// Excepción lanzada cuando no se encuentra el recurso solicitado.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException()
        : base("El recurso solicitado no fue encontrado.") { }

    public NotFoundException(string message)
        : base(message) { }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} con identificador '{key}' no fue encontrado.") { }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
