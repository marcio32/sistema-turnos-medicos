# AGENTS.md — Contexto para Asistentes de IA

## Stack Tecnológico

| Capa | Tecnologías |
|------|-------------|
| **Backend** | .NET 10, C#, Minimal APIs + Controllers |
| **ORM** | Entity Framework Core + SQL Server |
| **Cache** | Redis (IDistributedCache + Output Caching) |
| **Mensajería** | Amazon SQS (LocalStack en desarrollo) |
| **Real-time** | SignalR (hub `/hubs/turnos`) |
| **Frontend** | React 18 + Vite + TypeScript |
| **Estado cliente** | Zustand (stores por dominio) |
| **Server state** | TanStack Query (React Query v5) |
| **Formularios** | React Hook Form + Zod |
| **Estilos** | Tailwind CSS |
| **Testing frontend** | Vitest + Testing Library |
| **Testing backend** | xUnit + TestContainers |
| **Agente IA** | Anthropic Tool Use (Claude) |
| **Containerización** | Docker + docker-compose |
| **CI/CD** | GitHub Actions |

## Convenciones

### Respuestas de API — Unified Response Model (URM)

Todas las respuestas de la API usan `ApiResponse<T>`:

```csharp
public class ApiResponse<T>
{
    public T? Data { get; set; }
    public StatusInfo Status { get; set; }       // { Code, Message }
    public List<ErrorDetail> Errors { get; set; } // { Field, Message, Code }
    public MetadataInfo Metadata { get; set; }    // { Timestamp, Pagination? }
}
```

Nunca devolver datos directamente; siempre envolver en `ApiResponse<T>`.

### Validación

- **DTOs de entrada**: FluentValidation para reglas complejas, Data Annotations para reglas simples
- **Frontend**: Zod schemas que reflejan las mismas reglas del backend
- Validar siempre en ambas capas (frontend y backend)

### Organización del Frontend — Feature-Based

```
frontend/src/
├── features/          # Módulos de dominio
│   ├── turnos/        # Componentes, hooks, services de turnos
│   ├── medicos/       # Componentes, hooks, services de médicos
│   ├── auth/          # Login, registro, auth state
│   └── admin/         # Panel administrativo
├── shared/
│   ├── components/    # Button, Card, Modal, Badge, Spinner, ErrorMessage
│   ├── hooks/         # useToggle, useLocalStorage, useDebounce, useSignalR
│   ├── stores/        # useTurnosStore, useAuthStore (Zustand)
│   ├── services/      # API client base
│   └── utils/         # Helpers genéricos
```

### Organización del Backend — Arquitectura en Capas

```
backend/src/TurnosApi/
├── Controllers/       # Endpoints REST (heredan ControllerBase)
├── Services/          # Lógica de negocio (interfaces + implementaciones)
├── Repositories/      # Acceso a datos (Repository pattern)
├── Models/            # Entidades del dominio
├── DTOs/              # Objetos de transferencia (Request/Response)
├── Validators/        # FluentValidation validators
├── Middleware/        # Exception handler, logging
├── Extensions/        # Extension methods para DI y middleware
├── Hubs/              # SignalR hubs
├── Infrastructure/    # DbContext, SQS publisher, Redis config
└── Common/            # ApiResponse, enums compartidos
```

### Patrones de Diseño

- **Repository Pattern**: Interfaz por entidad (`ITurnoRepository`, `IMedicoRepository`, `IPacienteRepository`)
- **Unit of Work**: `IUnitOfWork` agrupa repositorios y gestiona transacciones con `SaveChangesAsync()`
- **Inyección de Dependencias**: Registrar todo en `Program.cs` via `ServiceCollectionExtensions`
- **BackgroundService**: Worker que consume cola SQS para notificaciones asíncronas

## Convenciones de Nomenclatura

| Contexto | Convención | Ejemplo |
|----------|-----------|---------|
| C# clases, propiedades, métodos | PascalCase | `TurnoService`, `CrearTurno()` |
| C# parámetros, variables locales | camelCase | `turnoId`, `fechaInicio` |
| C# interfaces | Prefijo I + PascalCase | `ITurnoRepository` |
| TypeScript funciones, variables | camelCase | `useTurnos`, `handleSubmit` |
| TypeScript tipos/interfaces | PascalCase | `TurnoFormData`, `ApiResponse` |
| React componentes | PascalCase | `TurnoCard`, `TurnoForm` |
| Archivos TypeScript | camelCase o PascalCase (componentes) | `turnosService.ts`, `TurnoCard.tsx` |
| Términos de dominio | Español | `Turno`, `Medico`, `Paciente`, `Especialidad` |
| Endpoints REST | kebab-case en plural | `/api/turnos`, `/api/medicos` |
| Mensajes SQS | dot notation | `turno.creado`, `turno.confirmado` |

## Restricciones — QUÉ NO HACER

- **NO introducir otro state manager**: Solo Zustand para estado cliente. No Redux, no Jotai, no Context API para estado global.
- **NO bypasear validación**: Todo DTO debe pasar por FluentValidation. Todo formulario debe validar con Zod.
- **NO usar SQL raw**: Siempre EF Core con LINQ. No `FromSqlRaw` ni `ExecuteSqlRaw` sin justificación extrema.
- **NO exponer detalles internos en respuestas de API**: No stack traces, no nombres de tablas, no connection strings en errores. Usar ProblemDetails (RFC 7807) para errores.
- **NO crear endpoints fuera de Controllers o Minimal APIs registrados**: Mantener consistencia en la capa de presentación.
- **NO hacer fetch directo con `fetch()` o `axios`**: Toda comunicación con el servidor pasa por TanStack Query (queries para lectura, mutations para escritura).
- **NO instalar dependencias sin versión fija**: Usar versiones exactas en `package.json` y `.csproj`.
- **NO commitear sin Conventional Commits**: Formato `tipo(scope): descripción` (feat, fix, chore, docs, test, refactor).
- **NO ignorar los estados async**: Todo componente que consume datos remotos debe manejar loading, error y empty states.
- **NO mutar estado directamente**: En Zustand usar `set()`, nunca mutar objetos del store.

## Guía para Generación de Código

Al generar código para este proyecto:

1. **Backend**: Seguir el flujo Controller → Service → Repository → Database
2. **Frontend**: Seguir el flujo Component → Hook (TanStack Query) → Service → API
3. **Errores**: Centralizar en `GlobalExceptionHandler`, devolver `ProblemDetails`
4. **Tests backend**: Integration tests con TestContainers (SQL Server + Redis reales)
5. **Tests frontend**: Vitest + Testing Library, patrón Arrange/Act/Assert
6. **Cache**: Usar `IDistributedCache` para Redis, Output Caching para GET endpoints
7. **Eventos**: Publicar a SQS en cambios de estado de turno, consumir en Worker
8. **Real-time**: Notificar cambios via SignalR hub a clientes conectados
