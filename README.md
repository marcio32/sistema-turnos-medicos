# Sistema de Gestión de Turnos Médicos

Sistema integral para la gestión de turnos médicos, desarrollado progresivamente a lo largo de 16 clases como proyecto unificador del curso de Desarrollo Full-Stack con IA.

## Descripción

Este proyecto implementa un sistema completo de gestión de turnos médicos que permite a pacientes reservar, confirmar y cancelar turnos con médicos de distintas especialidades. Incluye notificaciones en tiempo real, un agente de IA conversacional y observabilidad de punta a punta.

## Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| **Frontend** | React 18 + Vite + TypeScript, Tailwind CSS, Zustand, TanStack Query, React Hook Form + Zod, React Router v7, SignalR Client |
| **Backend** | .NET 10, Entity Framework Core, Redis, Amazon SQS (LocalStack), SignalR, OpenTelemetry, FluentValidation |
| **Agente IA** | TypeScript, Anthropic Tool Use (Claude), MCP Server |
| **Infraestructura** | Docker Compose, SQL Server, Redis, LocalStack, GitHub Actions CI |

## Estructura del Proyecto

```
sistema-turnos-medicos/
├── frontend/          # Aplicación React (Módulo 2)
├── backend/           # API .NET 10 + Worker Service (Módulo 3)
├── agent/             # Agente IA con Anthropic Tool Use (Módulo 4)
├── infra/             # Docker Compose y scripts de infraestructura
├── docs/              # Documentación del proyecto y guías por clase
├── .github/           # GitHub Actions workflows y templates
├── .eslintrc.json     # Configuración de ESLint
├── .prettierrc        # Configuración de Prettier
├── .commitlintrc.json # Configuración de Conventional Commits
└── README.md          # Este archivo
```

## Requisitos Previos

- [Node.js](https://nodejs.org/) v20 o superior
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)
- Cuenta en [GitHub](https://github.com/)

## Guía de Setup

### 1. Clonar el repositorio

```bash
git clone https://github.com/<tu-org>/sistema-turnos-medicos.git
cd sistema-turnos-medicos
```

### 2. Levantar la infraestructura

```bash
cd infra
docker compose up -d
```

Esto levanta: SQL Server, Redis y LocalStack (SQS).

### 3. Backend (.NET 10)

```bash
cd backend/src/TurnosApi
dotnet restore
dotnet ef database update
dotnet run
```

La API estará disponible en `http://localhost:5000`. Swagger UI en `http://localhost:5000/swagger`.

### 4. Frontend (React + Vite)

```bash
cd frontend
npm install
npm run dev
```

La aplicación estará disponible en `http://localhost:5173`.

### 5. Agente IA

```bash
cd agent
npm install
cp .env.example .env  # Configurar ANTHROPIC_API_KEY
npx tsx src/agent.ts
```

### 6. MCP Server (Model Context Protocol)

El sistema expone sus 5 tools como un servidor MCP, permitiendo que cualquier cliente compatible (Kiro, Claude Desktop, Cursor, etc.) se conecte directamente al backend de turnos.

#### Conexión rápida

El archivo `.mcp.json` en la raíz del proyecto ya tiene la configuración lista. Los clientes MCP que soporten auto-descubrimiento (como Kiro) detectarán el servidor automáticamente.

#### Conexión manual en Claude Desktop

Agregar esta entrada en el archivo de configuración de Claude Desktop (`claude_desktop_config.json`):

```json
{
  "mcpServers": {
    "turnos-medicos": {
      "command": "npx",
      "args": ["tsx", "./agent/src/mcp-server.ts"],
      "env": {
        "API_BASE_URL": "http://localhost:5000/api"
      }
    }
  }
}
```

> **Nota:** Asegurarse de que el backend esté corriendo en `localhost:5000` antes de conectar el cliente MCP.

#### Tools disponibles vía MCP

| Tool | Descripción |
|------|-------------|
| `consultar_disponibilidad` | Consulta horarios libres de un médico en una fecha |
| `reservar_turno` | Crea un nuevo turno médico |
| `cancelar_turno` | Cancela un turno existente |
| `listar_turnos_paciente` | Lista todos los turnos de un paciente |
| `enviar_recordatorio` | Envía un recordatorio via cola SQS |

#### Ejecutar el MCP server manualmente

```bash
cd agent
npm install
npx tsx src/mcp-server.ts
```

El servidor escucha por stdin/stdout siguiendo el protocolo JSON-RPC de MCP.

## Módulos del Curso

| Módulo | Clases | Tema |
|--------|--------|------|
| 1 | 1-4 | Git, CI/CD y Organización Ágil |
| 2 | 5-8 | Frontend React Profesional |
| 3 | 9-12 | Backend .NET 10 con Tecnologías Modernas |
| 4 | 13-16 | Desarrollo Asistido por IA y Agentes |

## Convenciones

- **Commits**: Conventional Commits (`feat:`, `fix:`, `docs:`, `chore:`, etc.)
- **Branches**: `feature/`, `fix/`, `docs/` seguido de descripción corta
- **Code Review**: Checklist obligatorio en cada Pull Request
- **Respuestas API**: Unified Response Model (URM) en todos los endpoints

## Licencia

Este proyecto es material educativo del curso. Uso exclusivamente académico.
