namespace TurnosApi.DTOs;

/// <summary>
/// Request para enviar un recordatorio de turno via cola SQS.
/// El Worker Service procesa el mensaje y simula el envío de la notificación.
/// </summary>
public class RecordatorioRequest
{
    /// <summary>
    /// ID del turno (también se recibe por ruta).
    /// </summary>
    public int TurnoId { get; set; }

    /// <summary>
    /// Mensaje personalizado del recordatorio. Si no se provee, se usa un mensaje por defecto.
    /// </summary>
    public string Mensaje { get; set; } = string.Empty;
}
