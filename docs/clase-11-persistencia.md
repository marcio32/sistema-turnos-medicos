# Clase 11: Persistencia con EF Core, Repository Pattern y SQS

## Objetivo

Integrar Entity Framework Core con SQL Server, implementar Repository + Unit of Work, modelar relaciones del dominio y configurar SQS (LocalStack) para notificaciones asíncronas.

## Duración estimada: 60 minutos

## Formato: Lab guiado

---

## Paso 1: Configurar EF Core con SQL Server (8 min)

Instalar paquetes: `Microsoft.EntityFrameworkCore.SqlServer`, `.Design`, `.Tools`.

Levantar SQL Server: `docker run -d --name sqlserver -e ACCEPT_EULA=Y -e SA_PASSWORD=YourStrong!Pass -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest`

Configurar connection string en `appsettings.Development.json`.

## Paso 2: Crear AppDbContext con relaciones (10 min)

Crear `Infrastructure/AppDbContext.cs` con DbSets para Turno, Medico, Paciente, Especialidad.

En `OnModelCreating` definir relaciones 1:N:
- Médico → muchos Turnos
- Paciente → muchos Turnos
- Especialidad → muchos Médicos

Agregar índices para consultas frecuentes (MedicoId + Fecha en Turnos).

## Paso 3: Migraciones y seed data (5 min)

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Incluir seed: 3 especialidades, 3 médicos, 5 pacientes de ejemplo.

## Paso 4: Repository pattern (10 min)

Crear interfaces `ITurnoRepository`, `IMedicoRepository`, `IPacienteRepository` con métodos: GetByIdAsync, GetAllAsync, AddAsync, UpdateAsync.

Implementar `TurnoRepository` con método clave: `GetByMedicoYFechaAsync(medicoId, fecha)` para validar solapamiento contra datos reales.

## Paso 5: Unit of Work (5 min)

Crear `IUnitOfWork` con propiedades: Turnos, Medicos, Pacientes y método `SaveChangesAsync()`.

Implementar sobre AppDbContext. Registrar: `AddScoped<IUnitOfWork, UnitOfWork>()`.

## Paso 6: Refactorizar TurnoService (10 min)

Reemplazar listas en memoria por llamadas a `_uow.Turnos`. Ahora las validaciones de solapamiento consultan la base real. Verificar que los datos persisten tras reinicio.

## Paso 7: SQS publisher + Worker (12 min)

Levantar LocalStack: `docker run -d --name localstack -p 4566:4566 localstack/localstack`

Crear cola: `aws --endpoint-url=http://localhost:4566 sqs create-queue --queue-name turnos-events`

Crear `Infrastructure/SqsPublisher.cs` que serialice `TurnoEvent` y envíe a la cola. Publicar en: CrearTurno, ConfirmarTurno, CancelarTurno.

Crear `BackgroundService` consumidor que loguea cada mensaje procesado.

---

## Entregable esperado

- Persistencia real en SQL Server (datos sobreviven al reinicio)
- Relaciones 1:N funcionando (Médico→Turnos, Paciente→Turnos, Especialidad→Médicos)
- Repository + Unit of Work implementados
- Flujo SQS: crear turno → publicar evento → Worker consume y loguea

## Conexión con la Clase 12

En la próxima clase se endurecerá la API con FluentValidation, ProblemDetails, Swagger, rate limiting, OpenTelemetry, SignalR y tests de integración con TestContainers.
