# Clase 10: Controllers CRUD, Lógica de Negocio, URM y Redis Cache

## Objetivo

Implementar el recurso Turno completo con CRUD, reglas de negocio en el service, respuestas estandarizadas con URM (Unified Response Model) y caching distribuido con Redis.

## Duración estimada: 40 minutos

## Formato: Challenge guiado

---

## Paso 1: Unified Response Model (5 min)

Crear `Common/ApiResponse.cs` con estructura: `Data`, `Status` (Code, Message), `Errors` (lista de Field/Message/Code), `Metadata` (Timestamp, Pagination).

Toda respuesta de la API se envuelve en este formato genérico `ApiResponse<T>`.

## Paso 2: TurnosController CRUD completo (10 min)

Expandir el controller con 5 endpoints:
- `GET /api/turnos` — listar con paginación (page, pageSize)
- `GET /api/turnos/{id}` — obtener por ID
- `POST /api/turnos` — crear turno
- `PUT /api/turnos/{id}` — actualizar
- `DELETE /api/turnos/{id}` — cancelar

Todas las respuestas envueltas en `ApiResponse<T>` con status y metadata.

## Paso 3: TurnoService con reglas de negocio (10 min)

Implementar `ITurnoService` con validaciones:
1. **No solapamiento**: no permitir turnos del mismo médico en rango [hora, hora+duración)
2. **Fecha futura**: turno.Fecha > hoy
3. **Horario laboral**: solo entre 8:00 y 20:00
4. **Duración válida**: exactamente 30 o 60 minutos

Si falla una regla, lanzar excepción tipada (`ConflictoTurnoException`).

## Paso 4: DTOs con Data Annotations (5 min)

Crear `DTOs/CrearTurnoRequest.cs` con: `[Required]` en campos obligatorios, `[Range(30,60)]` en duración, `[StringLength(500, MinimumLength=3)]` en motivo.

## Paso 5: Redis cache (10 min)

Instalar `Microsoft.Extensions.Caching.StackExchangeRedis`. Levantar Redis: `docker run -d --name redis -p 6379:6379 redis:alpine`.

En TurnoService cachear consultas frecuentes (turnos del día por médico) con TTL de 5 minutos. Invalidar cache al crear/modificar turnos.

---

## Entregable esperado

- Recurso Turno con CRUD completo (5 endpoints)
- Respuestas en formato URM (`ApiResponse<T>`)
- Reglas de negocio: solapamiento, fecha futura, horario, duración
- Redis cache funcional (instancia local con Docker)

## Conexión con la Clase 11

En la próxima clase se reemplazará la persistencia en memoria por EF Core + SQL Server, se implementará Repository + Unit of Work y se configurará mensajería con SQS.
