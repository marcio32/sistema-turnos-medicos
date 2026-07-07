# Clase 14 — Prompting Técnico con Framework RTC

## Objetivo

Aplicar el framework RTC (Rol-Tarea-Criterio) para generar código de calidad del Sistema de Turnos, validando cada output contra criterios de aceptación definidos previamente.

## Duración: 45 minutos

## Prerequisitos

- AGENTS.md y CONTEXT.md creados (Clase 13)
- Backend API con endpoints de turnos funcionando
- Frontend con componentes reutilizables (Button, Card, Badge, Modal)

---

## Paso a Paso

### Paso 1: Definir criterios ANTES de ejecutar prompts (5 min)

Para la feature "Confirmación de Turno E2E", definir criterios:
- **Backend:** PUT /api/turnos/{id}/confirmar — valida estado Pendiente, valida 24hs, publica SQS, notifica SignalR
- **Frontend:** Botón Confirmar en TurnoCard, feedback loading/success/error, invalida cache
- **Test:** Turno pendiente confirma OK (200), turno expirado retorna error (400)

### Paso 2: Prompt RTC #1 — Endpoint backend (12 min)

Escribir prompt con estructura:
- **ROL:** Desarrollador .NET 10 senior, patrones Repository + UoW, URM, FluentValidation, eventos SQS
- **TAREA:** Implementar PUT /api/turnos/{id}/confirmar (validar estado Pendiente, validar <24hs, cambiar estado, publicar SQS, notificar SignalR, devolver ApiResponse<TurnoResponse>)
- **CRITERIO:** DI, sin lógica en controller, excepciones tipadas, ProblemDetails en errores

Ejecutar y evaluar: ✅ cumple / ❌ no cumple / 🔧 requiere ajuste

### Paso 3: Prompt RTC #2 — Componente frontend (12 min)

- **ROL:** Desarrollador React/TypeScript senior, TanStack Query, Zustand, Tailwind
- **TAREA:** ConfirmarTurnoButton con useMutation, estados idle/loading/error, invalidación de cache, solo visible si turno está Pendiente
- **CRITERIO:** TypeScript estricto, sin any, errores del backend visibles al usuario

Ejecutar y evaluar contra criterios del Paso 1.

### Paso 4: Prompt RTC #3 — Test de integración (10 min)

- **ROL:** QA engineer, xUnit + TestContainers + WebApplicationFactory
- **TAREA:** Test exitoso (crear → confirmar < 24hs → 200) y test fallido (crear → simular >24hs → 400)
- **CRITERIO:** Tests independientes, DB real con TestContainers, verificar status HTTP + contenido URM

### Paso 5: Consolidar (6 min)

1. Revisar los 3 outputs contra criterios del Paso 1
2. Integrar código en el proyecto y ejecutar tests
3. Documentar: qué se aceptó, qué se ajustó por cada prompt

---

## Entregable Esperado

- 3 prompts RTC documentados con output y evaluación
- Endpoint `PUT /api/turnos/{id}/confirmar` funcionando
- Componente `ConfirmarTurnoButton` integrado
- Flujo confirmación E2E operativo: clic → backend → SQS → SignalR

## Conexión con Clase 15

En la próxima clase construirás un agente con Anthropic tool use que usará estos endpoints (incluido confirmar) como herramientas. El agente resolverá flujos en lenguaje natural — los endpoints validados hoy serán las tools que el agente invocará.
