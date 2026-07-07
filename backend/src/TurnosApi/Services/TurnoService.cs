using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using TurnosApi.Common;
using TurnosApi.DTOs;
using TurnosApi.Exceptions;
using TurnosApi.Hubs;
using TurnosApi.Infrastructure;
using TurnosApi.Models;
using TurnosApi.Repositories;

namespace TurnosApi.Services;

public class TurnoService : ITurnoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCache _cache;
    private readonly ISqsPublisher _sqsPublisher;
    private readonly IHubContext<TurnosHub> _hubContext;
    private readonly ILogger<TurnoService> _logger;

    private static readonly DistributedCacheEntryOptions DisponibilidadCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    private static readonly DistributedCacheEntryOptions TurnosCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
    };

    public TurnoService(IUnitOfWork unitOfWork, IDistributedCache cache, ISqsPublisher sqsPublisher, IHubContext<TurnosHub> hubContext, ILogger<TurnoService> logger)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _sqsPublisher = sqsPublisher;
        _hubContext = hubContext;
        _logger = logger;
    }

    // Helper: try cache get without failing if Redis is down
    private async Task<string?> TryGetCacheAsync(string key)
    {
        try { return await _cache.GetStringAsync(key); }
        catch (Exception ex) { _logger.LogWarning(ex, "Cache read failed for key {Key}", key); return null; }
    }

    // Helper: try cache set without failing if Redis is down
    private async Task TrySetCacheAsync(string key, string value, DistributedCacheEntryOptions options)
    {
        try { await _cache.SetStringAsync(key, value, options); }
        catch (Exception ex) { _logger.LogWarning(ex, "Cache write failed for key {Key}", key); }
    }

    // Helper: try cache remove without failing
    private async Task TryRemoveCacheAsync(string key)
    {
        try { await _cache.RemoveAsync(key); }
        catch (Exception ex) { _logger.LogWarning(ex, "Cache remove failed for key {Key}", key); }
    }

    // =========================================================================
    // MÁQUINA DE ESTADOS — Transiciones válidas
    // =========================================================================

    /// <summary>
    /// Determina si una transición de estado es válida según la máquina de estados del turno.
    /// Transiciones permitidas:
    ///   Pendiente → Confirmado
    ///   Pendiente → Cancelado
    ///   Confirmado → Cancelado
    ///   Confirmado → Completado
    ///   Confirmado → NoAsistio
    /// </summary>
    public static bool IsValidTransition(EstadoTurno currentState, EstadoTurno newState)
    {
        return (currentState, newState) switch
        {
            (EstadoTurno.Pendiente, EstadoTurno.Confirmado) => true,
            (EstadoTurno.Pendiente, EstadoTurno.Cancelado) => true,
            (EstadoTurno.Confirmado, EstadoTurno.Cancelado) => true,
            (EstadoTurno.Confirmado, EstadoTurno.Completado) => true,
            (EstadoTurno.Confirmado, EstadoTurno.NoAsistio) => true,
            _ => false
        };
    }

    // =========================================================================
    // CREAR TURNO
    // =========================================================================

    public async Task<ApiResponse<TurnoResponse>> CrearTurno(CrearTurnoRequest request)
    {
        // Validar duración (30 o 60)
        if (request.Duracion != 30 && request.Duracion != 60)
        {
            return ApiResponse<TurnoResponse>.Fail(
                "La duración debe ser exactamente 30 o 60 minutos.",
                "Duracion",
                "INVALID_DURATION",
                400);
        }

        // Validar fecha futura
        if (request.Fecha <= DateOnly.FromDateTime(DateTime.Now))
        {
            return ApiResponse<TurnoResponse>.Fail(
                "La fecha del turno debe ser una fecha futura.",
                "Fecha",
                "INVALID_DATE",
                400);
        }

        // Validar horario laboral 8:00 - 20:00
        var horaInicio = request.Hora;
        var horaFin = request.Hora.AddMinutes(request.Duracion);
        var inicioLaboral = new TimeOnly(8, 0);
        var finLaboral = new TimeOnly(20, 0);

        if (horaInicio < inicioLaboral || horaFin > finLaboral)
        {
            return ApiResponse<TurnoResponse>.Fail(
                "El turno debe estar dentro del horario laboral (08:00 a 20:00).",
                "Hora",
                "OUTSIDE_WORK_HOURS",
                400);
        }

        // Verificar no solapamiento
        var turnosExistentes = await _unitOfWork.Turnos.GetByMedicoYFechaAsync(request.MedicoId, request.Fecha);
        var turnosActivos = turnosExistentes
            .Where(t => t.Estado != EstadoTurno.Cancelado)
            .ToList();

        if (HaySolapamiento(turnosActivos, horaInicio, request.Duracion))
        {
            throw new ConflictoTurnoException(
                "El médico ya tiene un turno asignado en ese horario.");
        }

        // Crear el turno
        var turno = new Turno
        {
            PacienteId = request.PacienteId,
            MedicoId = request.MedicoId,
            Fecha = request.Fecha,
            Hora = request.Hora,
            Duracion = request.Duracion,
            Estado = EstadoTurno.Pendiente,
            Motivo = request.Motivo,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Turnos.AddAsync(turno);
        await _unitOfWork.SaveChangesAsync();

        // Invalidar cache relacionado
        await InvalidarCacheMedicoFecha(request.MedicoId, request.Fecha);

        // Publicar evento SQS
        await _sqsPublisher.PublishAsync(new TurnoEvent
        {
            Tipo = "turno.creado",
            TurnoId = turno.Id,
            PacienteId = turno.PacienteId,
            MedicoId = turno.MedicoId,
            Fecha = turno.Fecha,
            Hora = turno.Hora,
            Timestamp = DateTime.UtcNow
        });

        // Notificar clientes vía SignalR
        var turnoResponse = MapToResponse(turno);
        await _hubContext.Clients.All.SendAsync("TurnoCreado", turnoResponse);

        return ApiResponse<TurnoResponse>.Success(turnoResponse, "Turno creado exitosamente");
    }

    // =========================================================================
    // CONFIRMAR TURNO
    // =========================================================================

    public async Task<ApiResponse<TurnoResponse>> ConfirmarTurno(int turnoId)
    {
        var turno = await _unitOfWork.Turnos.GetByIdAsync(turnoId);
        if (turno is null)
        {
            throw new NotFoundException("Turno", turnoId);
        }

        // Validar transición de estado
        if (!IsValidTransition(turno.Estado, EstadoTurno.Confirmado))
        {
            throw new TransicionInvalidaException(turno.Estado, EstadoTurno.Confirmado);
        }

        // Validar confirmación dentro de 24 horas de creación
        if (DateTime.UtcNow - turno.CreatedAt > TimeSpan.FromHours(24))
        {
            return ApiResponse<TurnoResponse>.Fail(
                "La confirmación debe realizarse dentro de las 24 horas posteriores a la creación del turno.",
                "CreatedAt",
                "CONFIRMATION_EXPIRED",
                400);
        }

        turno.Estado = EstadoTurno.Confirmado;
        turno.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Turnos.UpdateAsync(turno);
        await _unitOfWork.SaveChangesAsync();

        // Invalidar cache relacionado
        await InvalidarCacheMedicoFecha(turno.MedicoId, turno.Fecha);

        // Publicar evento SQS
        await _sqsPublisher.PublishAsync(new TurnoEvent
        {
            Tipo = "turno.confirmado",
            TurnoId = turno.Id,
            PacienteId = turno.PacienteId,
            MedicoId = turno.MedicoId,
            Fecha = turno.Fecha,
            Hora = turno.Hora,
            Timestamp = DateTime.UtcNow
        });

        // Notificar clientes vía SignalR
        var turnoResponse = MapToResponse(turno);
        await _hubContext.Clients.All.SendAsync("TurnoConfirmado", turnoResponse);

        return ApiResponse<TurnoResponse>.Success(turnoResponse, "Turno confirmado exitosamente");
    }

    // =========================================================================
    // CANCELAR TURNO
    // =========================================================================

    public async Task<ApiResponse<TurnoResponse>> CancelarTurno(int turnoId)
    {
        var turno = await _unitOfWork.Turnos.GetByIdAsync(turnoId);
        if (turno is null)
        {
            throw new NotFoundException("Turno", turnoId);
        }

        // Validar transición de estado
        if (!IsValidTransition(turno.Estado, EstadoTurno.Cancelado))
        {
            throw new TransicionInvalidaException(turno.Estado, EstadoTurno.Cancelado);
        }

        // Validar al menos 2 horas de anticipación
        var fechaHoraTurno = turno.Fecha.ToDateTime(turno.Hora);
        if (fechaHoraTurno - DateTime.Now < TimeSpan.FromHours(2))
        {
            return ApiResponse<TurnoResponse>.Fail(
                "La cancelación requiere al menos 2 horas de anticipación respecto al horario del turno.",
                "Fecha",
                "CANCELLATION_TOO_LATE",
                400);
        }

        turno.Estado = EstadoTurno.Cancelado;
        turno.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Turnos.UpdateAsync(turno);
        await _unitOfWork.SaveChangesAsync();

        // Invalidar cache relacionado
        await InvalidarCacheMedicoFecha(turno.MedicoId, turno.Fecha);

        // Publicar evento SQS
        await _sqsPublisher.PublishAsync(new TurnoEvent
        {
            Tipo = "turno.cancelado",
            TurnoId = turno.Id,
            PacienteId = turno.PacienteId,
            MedicoId = turno.MedicoId,
            Fecha = turno.Fecha,
            Hora = turno.Hora,
            Timestamp = DateTime.UtcNow
        });

        // Notificar clientes vía SignalR
        var turnoResponse = MapToResponse(turno);
        await _hubContext.Clients.All.SendAsync("TurnoCancelado", turnoResponse);

        return ApiResponse<TurnoResponse>.Success(turnoResponse, "Turno cancelado exitosamente");
    }

    // =========================================================================
    // LISTAR TURNOS
    // =========================================================================

    public async Task<ApiResponse<List<TurnoResponse>>> ListarTurnos(TurnoFilter filter)
    {
        // Intentar obtener del cache para consultas frecuentes (sin filtros complejos)
        var cacheKey = $"turnos:list:{filter.Page}:{filter.PageSize}:{filter.MedicoId}:{filter.PacienteId}:{filter.Fecha}:{filter.Estado}";
        var cached = await TryGetCacheAsync(cacheKey);

        if (cached is not null)
        {
            var cachedData = JsonSerializer.Deserialize<List<TurnoResponse>>(cached);
            if (cachedData is not null)
            {
                return ApiResponse<List<TurnoResponse>>.Success(cachedData, "Turnos obtenidos exitosamente");
            }
        }

        var turnos = await _unitOfWork.Turnos.GetAllAsync(filter);
        var response = turnos.Select(MapToResponse).ToList();

        // Cachear resultado
        var serialized = JsonSerializer.Serialize(response);
        await TrySetCacheAsync(cacheKey, serialized, TurnosCacheOptions);

        return ApiResponse<List<TurnoResponse>>.Success(response, "Turnos obtenidos exitosamente");
    }

    // =========================================================================
    // CONSULTAR DISPONIBILIDAD
    // =========================================================================

    public async Task<ApiResponse<DisponibilidadResponse>> ConsultarDisponibilidad(int medicoId, DateOnly fecha)
    {
        // Intentar obtener del cache
        var cacheKey = $"disponibilidad:{medicoId}:{fecha:yyyy-MM-dd}";
        var cached = await TryGetCacheAsync(cacheKey);

        if (cached is not null)
        {
            var cachedData = JsonSerializer.Deserialize<DisponibilidadResponse>(cached);
            if (cachedData is not null)
            {
                return ApiResponse<DisponibilidadResponse>.Success(cachedData, "Disponibilidad consultada exitosamente");
            }
        }

        var medico = await _unitOfWork.Medicos.GetByIdAsync(medicoId);
        if (medico is null)
        {
            throw new NotFoundException("Médico", medicoId);
        }

        var turnosDelDia = await _unitOfWork.Turnos.GetByMedicoYFechaAsync(medicoId, fecha);
        var turnosActivos = turnosDelDia
            .Where(t => t.Estado != EstadoTurno.Cancelado)
            .ToList();

        var slotsDisponibles = CalcularSlotsDisponibles(turnosActivos);

        var response = new DisponibilidadResponse
        {
            MedicoId = medicoId,
            MedicoNombre = medico.Nombre,
            Fecha = fecha,
            SlotsDisponibles = slotsDisponibles
        };

        // Cachear resultado con TTL de 5 minutos
        var serialized = JsonSerializer.Serialize(response);
        await TrySetCacheAsync(cacheKey, serialized, DisponibilidadCacheOptions);

        return ApiResponse<DisponibilidadResponse>.Success(response, "Disponibilidad consultada exitosamente");
    }

    // =========================================================================
    // MÉTODOS PRIVADOS
    // =========================================================================

    /// <summary>
    /// Verifica si el nuevo turno se solapa con turnos existentes del mismo médico.
    /// </summary>
    private static bool HaySolapamiento(List<Turno> turnosExistentes, TimeOnly horaInicio, int duracion)
    {
        var finNuevo = horaInicio.AddMinutes(duracion);

        foreach (var turno in turnosExistentes)
        {
            var inicioExistente = turno.Hora;
            var finExistente = turno.Hora.AddMinutes(turno.Duracion);

            // Solapamiento: inicio < fin_existente AND fin > inicio_existente
            if (horaInicio < finExistente && finNuevo > inicioExistente)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Calcula los slots de 30 minutos disponibles entre 8:00 y 20:00,
    /// excluyendo los horarios ocupados por turnos existentes.
    /// </summary>
    private static List<TimeOnly> CalcularSlotsDisponibles(List<Turno> turnosActivos)
    {
        var slots = new List<TimeOnly>();
        var inicioLaboral = new TimeOnly(8, 0);
        var finLaboral = new TimeOnly(20, 0);
        var intervalo = TimeSpan.FromMinutes(30);

        var horaActual = inicioLaboral;

        while (horaActual < finLaboral)
        {
            var slotFin = horaActual.AddMinutes(30);

            // Verificar si este slot de 30 min está libre
            var ocupado = turnosActivos.Any(t =>
            {
                var turnoInicio = t.Hora;
                var turnoFin = t.Hora.AddMinutes(t.Duracion);
                return horaActual < turnoFin && slotFin > turnoInicio;
            });

            if (!ocupado)
            {
                slots.Add(horaActual);
            }

            horaActual = horaActual.Add(intervalo);
        }

        return slots;
    }

    /// <summary>
    /// Mapea un Turno a su DTO de respuesta.
    /// </summary>
    private static TurnoResponse MapToResponse(Turno turno)
    {
        return new TurnoResponse
        {
            Id = turno.Id,
            PacienteId = turno.PacienteId,
            PacienteNombre = turno.Paciente?.Nombre ?? string.Empty,
            MedicoId = turno.MedicoId,
            MedicoNombre = turno.Medico?.Nombre ?? string.Empty,
            Fecha = turno.Fecha,
            Hora = turno.Hora,
            Duracion = turno.Duracion,
            Estado = turno.Estado,
            Motivo = turno.Motivo,
            CreatedAt = turno.CreatedAt
        };
    }

    /// <summary>
    /// Invalida las entradas de cache relacionadas con un médico y fecha específicos.
    /// </summary>
    private async Task InvalidarCacheMedicoFecha(int medicoId, DateOnly fecha)
    {
        var disponibilidadKey = $"disponibilidad:{medicoId}:{fecha:yyyy-MM-dd}";
        await TryRemoveCacheAsync(disponibilidadKey);
    }
}
