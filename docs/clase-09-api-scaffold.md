# Clase 9: Scaffold API .NET 10 con Capas y Minimal APIs

## Objetivo

Crear la estructura profesional de la API "TurnosApi" en .NET 10 con arquitectura en capas, inyección de dependencias, Controllers + Minimal APIs y Dockerfile multi-stage.

## Duración estimada: 50 minutos

## Formato: Live coding (30 min) + ejercicio guiado (20 min)

---

## Paso 1: Crear el proyecto .NET 10 (5 min)

```bash
dotnet new webapi -n TurnosApi --framework net10.0
```

Verificar que corre con `dotnet run` y responde en `http://localhost:5000`.

## Paso 2: Estructura de capas (5 min)

Crear carpetas: `Controllers/`, `Services/`, `Repositories/`, `Models/`, `DTOs/`, `Middleware/`, `Extensions/`, `Infrastructure/`, `Common/`, `Hubs/`.

Explicar responsabilidad: Controllers (entrada HTTP), Services (lógica), Repositories (datos).

## Paso 3: Modelo de dominio básico (5 min)

Crear `Models/Turno.cs` (Id, PacienteId, MedicoId, Fecha, Hora, Duracion, Estado, Motivo).
Crear `Models/EstadoTurno.cs` enum (Pendiente, Confirmado, Cancelado, Completado, NoAsistio).
Crear `DTOs/TurnoResponse.cs` con campos de salida.

## Paso 4: Inyección de dependencias (5 min)

En `Program.cs` registrar servicios: `AddScoped<ITurnoService, TurnoService>()`, `AddScoped<ITurnoRepository, TurnoRepository>()`. Configurar `appsettings.json` con secciones: ConnectionStrings, Redis, SQS.

## Paso 5: Controller tradicional (10 min)

Crear `Controllers/TurnosController.cs` con `[ApiController]`:
- `GET /api/turnos` — listar (datos en memoria)
- `GET /api/turnos/{id}` — obtener por ID
- `POST /api/turnos` — crear turno

Inyectar `ITurnoService` en constructor. Devolver datos hardcodeados.

## Paso 6: Minimal APIs (5 min)

En `Program.cs` agregar endpoints equivalentes:

```csharp
app.MapGet("/api/v2/turnos", () => Results.Ok(listaTurnos));
app.MapPost("/api/v2/turnos", (CrearTurnoRequest req) => Results.Created(...));
```

Comparar ambos enfoques: cuándo usar cada uno.

## Paso 7: Health Checks (5 min)

Agregar `builder.Services.AddHealthChecks()` y `app.MapHealthChecks("/health")`. Verificar que `GET /health` retorna Healthy.

## Paso 8: Dockerfile multi-stage (10 min)

Crear Dockerfile con etapa build (sdk:10.0) y etapa runtime (aspnet:10.0). Construir: `docker build -t turnos-api .` y probar: `docker run -p 5000:8080 turnos-api`.

---

## Entregable esperado

- API corriendo en localhost con `GET /api/turnos` funcional (datos en memoria)
- Estructura de capas completa
- Health check en `/health`
- Dockerfile multi-stage funcional

## Conexión con la Clase 10

En la próxima clase se implementará CRUD completo con reglas de negocio (solapamiento, horarios), URM y caching con Redis sobre esta estructura.
