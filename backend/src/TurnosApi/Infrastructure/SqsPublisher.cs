using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using TurnosApi.Models;

namespace TurnosApi.Infrastructure;

/// <summary>
/// Implementación de ISqsPublisher que envía eventos TurnoEvent a una cola SQS.
/// Usa AWSSDK.SQS con configuración de LocalStack para desarrollo local.
/// </summary>
public class SqsPublisher : ISqsPublisher
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl;
    private readonly ILogger<SqsPublisher> _logger;

    public SqsPublisher(IAmazonSQS sqsClient, IConfiguration configuration, ILogger<SqsPublisher> logger)
    {
        _sqsClient = sqsClient;
        _logger = logger;

        var serviceUrl = configuration["Sqs:ServiceUrl"] ?? "http://localhost:4566";
        var queueName = configuration["Sqs:QueueName"] ?? "turnos-events";
        _queueUrl = $"{serviceUrl}/000000000000/{queueName}";
    }

    public async Task PublishAsync(TurnoEvent evt)
    {
        var messageBody = JsonSerializer.Serialize(evt, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var request = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = messageBody
        };

        try
        {
            var response = await _sqsClient.SendMessageAsync(request);
            _logger.LogInformation(
                "Evento SQS publicado: {Tipo} | TurnoId: {TurnoId} | MessageId: {MessageId}",
                evt.Tipo, evt.TurnoId, response.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al publicar evento SQS: {Tipo} | TurnoId: {TurnoId}",
                evt.Tipo, evt.TurnoId);
            // No lanzamos la excepción para no interrumpir el flujo principal
            // El evento se perderá pero el turno ya fue procesado exitosamente
        }
    }
}
