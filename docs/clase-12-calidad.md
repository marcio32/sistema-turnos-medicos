# Clase 12: Validaciones, Errores, Observabilidad y Testing

## Objetivo

Endurecer la API con FluentValidation, manejo centralizado de errores con ProblemDetails, Swagger, rate limiting, OpenTelemetry, hub SignalR y tests de integración con TestContainers.

## Duración estimada: 50 minutos

## Formato: Challenge guiado

---

## Paso 1: FluentValidation (7 min)

Crear `Validators/CrearTurnoValidator.cs` con reglas: fecha futura, hora 8-20, duración 30|60, y validación async de que el médico exista consultando el repositorio.

Registrar: `builder.Services.AddValidatorsFromAssemblyContaining<CrearTurnoValidator>()`.

## Paso 2: Middleware de errores con ProblemDetails (7 min)

Crear `GlobalExceptionHandler` implementando `IExceptionHandler`. Mapear excepciones a status codes: ValidationException→400, NotFoundException→404, ConflictoTurnoException→409, genérica→500.

Devolver `ProblemDetails` (RFC 7807) con Status, Title y Detail.

## Paso 3: Swagger/OpenAPI (5 min)

Configurar `AddSwaggerGen` con XML comments. Agregar atributos `[ProducesResponseType]` en controllers para documentar 200, 400, 404, 409. Verificar UI en `/swagger`.

## Paso 4: Rate Limiting (5 min)

Configurar dos políticas con el middleware nativo:
- `"publico"`: fixed window, 100 req/min
- `"autenticado"`: sliding window, 500 req/min, 4 segmentos

Aplicar con `[EnableRateLimiting("publico")]` en endpoints.

## Paso 5: OpenTelemetry (8 min)

Instalar paquetes: `OpenTelemetry.Extensions.Hosting`, `.Instrumentation.AspNetCore`, `.Exporter.Console`.

Configurar trazas (ASP.NET Core, HttpClient, EF Core) y métricas con exportación a consola. Crear métrica custom: contador de turnos creados.

## Paso 6: Hub SignalR (8 min)

Crear `Hubs/TurnosHub.cs` con métodos: NotificarTurnoCreado, NotificarTurnoCancelado.

Registrar: `AddSignalR()` + `MapHub<TurnosHub>("/hubs/turnos")`.

Inyectar `IHubContext<TurnosHub>` en TurnoService para notificar en cada cambio de estado.

## Paso 7: Integration Tests con TestContainers (10 min)

Crear proyecto xUnit con `Testcontainers.MsSql` y `Microsoft.AspNetCore.Mvc.Testing`.

Test 1: `CrearTurno_DatosValidos_Retorna201` — crear turno y verificar persistencia.
Test 2: `CrearTurno_Solapado_Retorna409` — crear dos turnos solapados y verificar rechazo.

---

## Entregable esperado

- FluentValidation con reglas complejas funcionando
- Errores centralizados con ProblemDetails y status codes correctos
- Swagger documentando todos los endpoints
- Rate limiting activo (100/500 req/min)
- OpenTelemetry exportando trazas a consola
- SignalR hub funcional en `/hubs/turnos`
- 2 integration tests pasando con TestContainers

## Conexión con la Clase 13

En la próxima clase (Módulo 4) se usará esta API completa como fuente de herramientas para un agente de IA. Los endpoints documentados en Swagger servirán de referencia para definir las tools con Anthropic tool use.
