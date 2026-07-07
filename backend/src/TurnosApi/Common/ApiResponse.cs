namespace TurnosApi.Common;

/// <summary>
/// Modelo de Respuesta Unificado (URM) — envuelve todas las respuestas de la API
/// con una estructura consistente: data, status, errors y metadata.
/// </summary>
public class ApiResponse<T>
{
    public T? Data { get; set; }
    public StatusInfo Status { get; set; } = new();
    public List<ErrorDetail> Errors { get; set; } = new();
    public MetadataInfo Metadata { get; set; } = new();

    /// <summary>
    /// Crea una respuesta exitosa con datos.
    /// </summary>
    public static ApiResponse<T> Success(T data, string message = "Operación exitosa")
    {
        return new ApiResponse<T>
        {
            Data = data,
            Status = new StatusInfo { Code = 200, Message = message },
            Errors = new List<ErrorDetail>(),
            Metadata = new MetadataInfo()
        };
    }

    /// <summary>
    /// Crea una respuesta exitosa con datos y paginación.
    /// </summary>
    public static ApiResponse<T> Success(T data, PaginationInfo pagination, string message = "Operación exitosa")
    {
        return new ApiResponse<T>
        {
            Data = data,
            Status = new StatusInfo { Code = 200, Message = message },
            Errors = new List<ErrorDetail>(),
            Metadata = new MetadataInfo { Pagination = pagination }
        };
    }

    /// <summary>
    /// Crea una respuesta de error con lista de errores.
    /// </summary>
    public static ApiResponse<T> Fail(List<ErrorDetail> errors, int code = 400, string message = "Error en la operación")
    {
        return new ApiResponse<T>
        {
            Data = default,
            Status = new StatusInfo { Code = code, Message = message },
            Errors = errors,
            Metadata = new MetadataInfo()
        };
    }

    /// <summary>
    /// Crea una respuesta de error con un solo mensaje.
    /// </summary>
    public static ApiResponse<T> Fail(string errorMessage, string field = "", string code = "VALIDATION_ERROR", int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Data = default,
            Status = new StatusInfo { Code = statusCode, Message = "Error en la operación" },
            Errors = new List<ErrorDetail>
            {
                new ErrorDetail { Field = field, Message = errorMessage, Code = code }
            },
            Metadata = new MetadataInfo()
        };
    }
}

public class StatusInfo
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ErrorDetail
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class MetadataInfo
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public PaginationInfo? Pagination { get; set; }
}

public class PaginationInfo
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
