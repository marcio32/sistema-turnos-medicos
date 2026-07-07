# Clase 16 — MCP Server, Producción y Demo del Proyecto Integrador

## Objetivo

Extender el agente con MCP (Model Context Protocol), agregar consideraciones de producción (límite de costo, fallback, permisos mínimos), y presentar el proyecto integrador al grupo con demo en vivo.

## Duración: 60 minutos (30 min desarrollo + 30 min presentaciones)

## Prerequisitos

- Agente funcional de Clase 15 con 4+ tools y loop agéntico
- Backend + Worker Service + Frontend corriendo
- Docker Compose levantado (SQL Server, Redis, LocalStack)

---

## Bloque Desarrollo (30 min)

### Paso 1: Tool con efecto secundario — enviar_recordatorio (8 min)

1. Verificar tool `enviar_recordatorio` en `agent/src/tools.ts` (POST que publica a cola SQS)
2. Ejecutar: "Enviá un recordatorio al paciente del turno 42"
3. Confirmar en logs del Worker Service: `[Worker] Mensaje recibido: recordatorio turno #42`
4. Este es un efecto observable — prueba que el agente produce acciones reales

### Paso 2: Consideraciones de producción (10 min)

Implementar en el agente:

1. **Límite de tokens:** Máximo 4096 tokens por ejecución. Si se supera, cortar con mensaje amigable.
2. **Fallback de errores:** Si el backend no responde, devolver `{ error: true, message: "Servicio no disponible" }` en vez de crashear.
3. **Permisos mínimos:**
   - Tools de lectura (consultar_disponibilidad, listar_turnos): ejecución directa
   - Tools de escritura (reservar, cancelar, recordatorio): log de advertencia + validación de input antes de ejecutar

### Paso 3: Exponer como MCP Server (7 min)

1. Revisar `agent/src/mcp-server.ts`: expone las 5 tools via protocolo MCP (stdio)
2. Verificar configuración MCP en la raíz:
   ```json
   { "mcpServers": { "turnos-medicos": {
     "command": "npx", "args": ["tsx", "./agent/src/mcp-server.ts"],
     "env": { "API_BASE_URL": "http://localhost:5000" }
   }}}
   ```
3. Probar conexión desde un cliente MCP-compatible (Claude Desktop, Kiro, etc.)

### Paso 4: Preparar demo (5 min)

Verificar stack completo corriendo y preparar flujo:
- Agente recibe "Reservar turno con Dr. García mañana 10hs"
- Agente consulta disponibilidad → reserva → envía recordatorio
- Worker procesa mensaje SQS
- Frontend recibe notificación SignalR

---

## Bloque Presentaciones (30 min)

### Formato: 5 minutos por equipo

1. **Problema** (30 seg): qué resuelve el agente
2. **Demo en vivo** (2 min): flujo completo prompt → agente → backend → notificación
3. **Decisiones técnicas** (1.5 min): tools elegidas, límites, manejo de errores
4. **Aprendizajes** (1 min): dificultades, sorpresas del modelo, qué cambiarían

---

## Entregable Esperado

- Tool `enviar_recordatorio` con efecto observable en Worker
- Consideraciones de producción implementadas (tokens, fallback, permisos)
- MCP Server configurado y verificable
- Demo presentada: lenguaje natural → agente → backend → SQS → Worker → SignalR → frontend

## Cierre del Curso

El sistema completo cubre el ciclo de vida de un proyecto real: organización (M1), frontend (M2), backend (M3), IA aplicada (M4). El agente no reemplaza al developer — lo potencia. La skill más valiosa: saber cuándo aceptar, corregir o rechazar el output de una IA.
