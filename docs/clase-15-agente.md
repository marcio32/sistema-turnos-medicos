# Clase 15 — Agente con Tool Use sobre el Backend de Turnos

## Objetivo

Construir un agente con Anthropic tool use que utilice los endpoints del Sistema de Turnos como herramientas, implementando el loop agéntico completo: definir tools → llamar modelo → ejecutar tool_use → devolver tool_result → iterar.

## Duración: 60 minutos

## Prerequisitos

- Backend API corriendo en `localhost:5000` con datos seed (médicos, pacientes, turnos)
- Node.js 18+ y `npm install` ejecutado en carpeta `agent/`
- Variable de entorno `ANTHROPIC_API_KEY` configurada

---

## Paso a Paso

### Paso 1: Setup del proyecto (8 min)

1. Ir a carpeta `agent/` e instalar dependencias: `npm install`
2. Crear `.env` con: `ANTHROPIC_API_KEY`, `API_BASE_URL=http://localhost:5000`, `MAX_STEPS=5`
3. Verificar backend: `curl http://localhost:5000/health`

### Paso 2: Definir tools basadas en endpoints (12 min)

Revisar `agent/src/tools.ts` — 5 tools mapeadas a endpoints reales:
- `consultar_disponibilidad` → GET /api/medicos/{id}/disponibilidad?fecha=
- `reservar_turno` → POST /api/turnos
- `cancelar_turno` → DELETE /api/turnos/{id}
- `listar_turnos_paciente` → GET /api/turnos?pacienteId=
- `enviar_recordatorio` → POST /api/notificaciones/recordatorio

Cada tool tiene `name`, `description`, `input_schema`. La función `executeTool(name, input)` hace fetch al backend. Probar una tool manualmente para verificar conectividad.

### Paso 3: Implementar loop agéntico (20 min)

Abrir `agent/src/loop.ts` y estudiar `runAgent(userMessage, maxSteps=5)`:

1. Enviar mensaje + tools al modelo Anthropic
2. Si `stop_reason === "tool_use"`: ejecutar tool contra backend, devolver resultado al modelo
3. Si respuesta es texto: retornar como respuesta final
4. Repetir hasta max 5 pasos

Implementar logging en cada paso:
- `[Paso N] Tool: {nombre} | Input: {JSON}`
- `[Paso N] Resultado: {resumen del backend}`
- `[Paso N] Pasos restantes: {N}`

### Paso 4: Ejecutar flujo de prueba (12 min)

Ejecutar: `npx tsx src/agent.ts`

Flujo principal:
```
> Quiero sacar un turno con el Dr. García para mañana a las 10.
> Si no hay disponibilidad, mostrame los horarios libres.
```

Observar: Paso 1 consulta disponibilidad → Paso 2 reserva o muestra alternativas → Paso 3 respuesta final.

Probar también: "¿Qué turnos tiene María López?" y "Cancelame el turno 42"

### Paso 5: Verificar robustez (8 min)

- Backend apagado → agente da mensaje amigable (fallback)
- Prompt ambiguo → nunca supera 5 pasos
- Logs claros en cada iteración del loop

---

## Entregable Esperado

- Agente funcional que resuelve reserva de turno end-to-end
- 4+ tools conectadas a endpoints reales del backend
- Loop con logging de cada paso y criterio de parada (máx 5)
- Al menos 2 flujos de prueba ejecutados exitosamente

## Conexión con Clase 16

En la próxima clase extenderás el agente con enviar_recordatorio (efecto SQS observable), agregarás límites de producción (tokens, permisos, fallback) y lo expondrás como MCP server para clientes externos. Cerrarás con la demo al grupo.
