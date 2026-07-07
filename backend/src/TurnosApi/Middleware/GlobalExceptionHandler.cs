using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TurnosApi.Exceptions;

namespace TurnosApi.Middleware;

/// <summary>
/// Manejador global de excepciones que implementa IExceptionHandler.
/// Mapea excepciones de dominio a respuestas ProblemDetails (RFC 7807)
/// con status codes apropiados.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Excepción no controlada: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            ValidationException validationEx => CreateValidationProblemDetails(validationEx, httpContext),
            NotFoundException notFoundEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                Title = "Recurso no encontrado",
                Status = StatusCodes.Status404NotFound,
                Detail = notFoundEx.Message,
                Instance = httpContext.Request.Path
            },
            ConflictoTurnoException conflictoEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                Title = "Conflicto de turno",
                Status = StatusCodes.Status409Conflict,
                Detail = conflictoEx.Message,
                Instance = httpContext.Request.Path
            },
            TransicionInvalidaException transicionEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                Title = "Transición de estado inválida",
                Status = StatusCodes.Status409Conflict,
                Detail = transicionEx.Message,
                Instance = httpContext.Request.Path
            },
            _ => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                Title = "Error interno del servidor",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Ocurrió un error inesperado. Intente nuevamente más tarde.",
                Instance = httpContext.Request.Path
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateValidationProblemDetails(
        ValidationException exception,
        HttpContext httpContext)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
            Title = "Error de validación",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Uno o más errores de validación ocurrieron.",
            Instance = httpContext.Request.Path
        };

        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        problemDetails.Extensions["errors"] = errors;

        return problemDetails;
    }
}
