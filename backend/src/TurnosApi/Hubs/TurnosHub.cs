using Microsoft.AspNetCore.SignalR;
using TurnosApi.DTOs;

namespace TurnosApi.Hubs;

/// <summary>
/// SignalR Hub para notificaciones en tiempo real de turnos médicos.
/// Los clientes pueden suscribirse para recibir eventos de cambios de estado.
/// </summary>
public class TurnosHub : Hub
{
    /// <summary>
    /// Notifica a todos los clientes conectados que se creó un nuevo turno.
    /// </summary>
    public async Task TurnoCreado(TurnoResponse turno)
    {
        await Clients.All.SendAsync("TurnoCreado", turno);
    }

    /// <summary>
    /// Notifica a todos los clientes conectados que un turno fue confirmado.
    /// </summary>
    public async Task TurnoConfirmado(TurnoResponse turno)
    {
        await Clients.All.SendAsync("TurnoConfirmado", turno);
    }

    /// <summary>
    /// Notifica a todos los clientes conectados que un turno fue cancelado.
    /// </summary>
    public async Task TurnoCancelado(TurnoResponse turno)
    {
        await Clients.All.SendAsync("TurnoCancelado", turno);
    }

    /// <summary>
    /// Notifica a todos los clientes conectados que un turno fue actualizado.
    /// </summary>
    public async Task TurnoActualizado(TurnoResponse turno)
    {
        await Clients.All.SendAsync("TurnoActualizado", turno);
    }
}
