using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using System.Diagnostics.Metrics;
using System.Threading.RateLimiting;
using TurnosApi.Hubs;
using TurnosApi.Infrastructure;
using TurnosApi.Middleware;
using TurnosApi.Repositories;
using TurnosApi.Services;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// SERVICES REGISTRATION (Dependency Injection)
// =============================================================================

// --- OpenAPI (built-in .NET 10 + Scalar UI) ---
builder.Services.AddOpenApi();

// --- Controllers ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serializar enums como strings (no como integers)
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter(
                System.Text.Json.JsonNamingPolicy.CamelCase));
    });

// --- SignalR ---
builder.Services.AddSignalR();

// --- Health Checks ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
var redisConfiguration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";

builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString, name: "sqlserver", tags: new[] { "db", "sql" })
    .AddRedis(redisConfiguration, name: "redis", tags: new[] { "cache", "redis" })
    .AddCheck("sqs", () =>
    {
        // Custom health check para SQS/LocalStack — verifica que la URL esté configurada
        var sqsUrl = builder.Configuration["Sqs:ServiceUrl"];
        return string.IsNullOrEmpty(sqsUrl)
            ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("SQS ServiceUrl no configurada")
            : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy($"SQS configurado en {sqsUrl}");
    }, tags: new[] { "messaging", "sqs" });

// --- Entity Framework Core ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Redis Cache ---
builder.Services.AddRedisCache(builder.Configuration);

// --- Output Caching ---
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(5)));

    // Políticas por recurso
    options.AddPolicy("TurnosPolicy", policy =>
        policy.Expire(TimeSpan.FromSeconds(60)));

    options.AddPolicy("MedicosPolicy", policy =>
        policy.Expire(TimeSpan.FromSeconds(120)));

    options.AddPolicy("DisponibilidadPolicy", policy =>
        policy.Expire(TimeSpan.FromSeconds(30)));
});

// --- Rate Limiting ---
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    options.AddSlidingWindowLimiter("sliding", limiterOptions =>
    {
        limiterOptions.PermitLimit = 500;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.SegmentsPerWindow = 5;
        limiterOptions.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// --- FluentValidation ---
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// --- OpenTelemetry ---
// Métricas custom del dominio
var turnosMeter = new Meter("TurnosApi.Metrics", "1.0.0");
var turnosCreadosCounter = turnosMeter.CreateCounter<long>("turnos.creados", "turnos", "Cantidad de turnos creados");
var tiempoRespuestaHistogram = turnosMeter.CreateHistogram<double>("turnos.tiempo_respuesta", "ms", "Tiempo de respuesta de operaciones de turnos");

builder.Services.AddSingleton(turnosMeter);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("TurnosApi")
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddMeter("TurnosApi.Metrics")
        .AddConsoleExporter());

// --- Repositories ---
builder.Services.AddScoped<ITurnoRepository, TurnoRepository>();
builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --- Services ---
builder.Services.AddScoped<ITurnoService, TurnoService>();
builder.Services.AddScoped<IMedicoService, MedicoService>();

// --- AWS SQS ---
builder.Services.AddSingleton<Amazon.SQS.IAmazonSQS>(sp =>
{
    var config = new Amazon.SQS.AmazonSQSConfig
    {
        ServiceURL = builder.Configuration["Sqs:ServiceUrl"] ?? "http://localhost:4566"
    };
    return new Amazon.SQS.AmazonSQSClient("test", "test", config);
});
builder.Services.AddScoped<ISqsPublisher, SqsPublisher>();

// --- Exception Handler ---
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Frontend Vite dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});

// =============================================================================
// BUILD APPLICATION
// =============================================================================

var app = builder.Build();

// =============================================================================
// MIDDLEWARE PIPELINE
// =============================================================================

// --- Exception Handling ---
app.UseExceptionHandler();

// --- OpenAPI + Scalar UI (solo en Development) ---
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Turnos API";
        options.Theme = ScalarTheme.BluePlanet;
    });
}

// --- CORS (debe ir ANTES de HTTPS redirect para que preflight no sea redirigido) ---
app.UseCors();

// --- HTTPS Redirection (solo en producción, en dev el frontend llama por HTTP) ---
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// --- Rate Limiting ---
app.UseRateLimiter();

// --- Output Caching ---
app.UseOutputCache();

// --- Authentication & Authorization ---
// TODO: Agregar autenticación/autorización cuando se implemente
// app.UseAuthentication();
// app.UseAuthorization();

// =============================================================================
// ENDPOINT MAPPING
// =============================================================================

// --- Health Check ---
app.MapHealthChecks("/health");

// --- Controllers (traditional REST approach) ---
app.MapControllers();

// --- SignalR Hubs ---
app.MapHub<TurnosHub>("/hubs/turnos");

// --- Minimal APIs (ejemplo para mostrar ambos enfoques — Req 7.5) ---
app.MapGet("/api/minimal/turnos", () =>
{
    // Ejemplo de Minimal API endpoint — se implementará con datos reales más adelante
    return Results.Ok(new
    {
        Data = new[] {
            new { Id = 1, Paciente = "Juan Pérez", Medico = "Dra. García", Estado = "Pendiente" },
            new { Id = 2, Paciente = "María López", Medico = "Dr. Rodríguez", Estado = "Confirmado" }
        },
        Status = new { Code = 200, Message = "OK" },
        Metadata = new { Timestamp = DateTime.UtcNow }
    });
})
.WithName("GetTurnosMinimal")
.WithTags("Turnos (Minimal API)")
.WithDescription("Ejemplo de Minimal API para listar turnos — demuestra ambos enfoques (Controllers + Minimal APIs)")
.CacheOutput("TurnosPolicy");

app.MapGet("/api/minimal/turnos/{id:int}", (int id) =>
{
    // Ejemplo de Minimal API con parámetro de ruta
    return Results.Ok(new
    {
        Data = new { Id = id, Paciente = "Juan Pérez", Medico = "Dra. García", Estado = "Pendiente" },
        Status = new { Code = 200, Message = "OK" },
        Metadata = new { Timestamp = DateTime.UtcNow }
    });
})
.WithName("GetTurnoByIdMinimal")
.WithTags("Turnos (Minimal API)")
.WithDescription("Ejemplo de Minimal API para obtener un turno por ID")
.CacheOutput("TurnosPolicy");

// =============================================================================
// RUN APPLICATION
// =============================================================================

app.Run();
