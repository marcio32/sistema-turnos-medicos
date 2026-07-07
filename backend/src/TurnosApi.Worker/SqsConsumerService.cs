using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace TurnosApi.Worker;

/// <summary>
/// BackgroundService que consume mensajes de la cola SQS de turnos.
/// Realiza polling continuo, deserializa TurnoEvent, loguea el procesamiento
/// y elimina el mensaje de la cola.
/// </summary>
public class SqsConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<SqsConsumerService> _logger;
    private readonly string _queueUrl;
    private readonly int _waitTimeSeconds = 20; // Long polling

    public SqsConsumerService(IAmazonSQS sqsClient, IConfiguration configuration, ILogger<SqsConsumerService> logger)
    {
        _sqsClient = sqsClient;
        _logger = logger;

        var serviceUrl = configuration["Sqs:ServiceUrl"] ?? "http://localhost:4566";
        var queueName = configuration["Sqs:QueueName"] ?? "turnos-events";
        _queueUrl = $"{serviceUrl}/000000000000/{queueName}";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SQS Consumer iniciado. Escuchando cola: {QueueUrl}", _queueUrl);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var request = new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = _waitTimeSeconds
                };

                var response = await _sqsClient.ReceiveMessageAsync(request, stoppingToken);

                foreach (var message in response.Messages)
                {
                    await ProcessMessageAsync(message, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Shutdown graceful
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recibir mensajes de SQS. Reintentando en 5 segundos...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("SQS Consumer detenido.");
    }

    private async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Mensaje recibido | MessageId: {MessageId}", message.MessageId);

            var turnoEvent = JsonSerializer.Deserialize<TurnoEventMessage>(message.Body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (turnoEvent is not null)
            {
                _logger.LogInformation(
                    "Evento procesado: {Tipo} | TurnoId: {TurnoId} | PacienteId: {PacienteId} | MedicoId: {MedicoId} | Fecha: {Fecha} | Hora: {Hora}",
                    turnoEvent.Tipo, turnoEvent.TurnoId, turnoEvent.PacienteId,
                    turnoEvent.MedicoId, turnoEvent.Fecha, turnoEvent.Hora);

                // Aquí se puede agregar lógica adicional:
                // - Enviar email/SMS de notificación
                // - Actualizar dashboards
                // - Disparar webhooks
            }

            // Eliminar mensaje de la cola tras procesamiento exitoso
            await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest
            {
                QueueUrl = _queueUrl,
                ReceiptHandle = message.ReceiptHandle
            }, cancellationToken);

            _logger.LogInformation("Mensaje eliminado de la cola | MessageId: {MessageId}", message.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar mensaje | MessageId: {MessageId}", message.MessageId);
            // El mensaje permanece en la cola y será reintentado tras el visibility timeout
        }
    }
}

/// <summary>
/// DTO para deserializar los eventos de turno recibidos de SQS.
/// </summary>
public class TurnoEventMessage
{
    public string Tipo { get; set; } = string.Empty;
    public int TurnoId { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string Hora { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}
