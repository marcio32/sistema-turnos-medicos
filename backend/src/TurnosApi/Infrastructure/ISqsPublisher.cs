using TurnosApi.Models;

namespace TurnosApi.Infrastructure;

/// <summary>
/// Interfaz para publicar eventos de turno a la cola SQS.
/// </summary>
public interface ISqsPublisher
{
    Task PublishAsync(TurnoEvent evt);
}
