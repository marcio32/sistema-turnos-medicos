# Clase 13 — Vibe Coding: Desarrollar con IA sobre el Sistema de Turnos

## Objetivo

Aplicar Context Engineering creando archivos de contexto (AGENTS.md, CONTEXT.md) y resolver una feature completa asistidos por IA con el loop especificar → generar → revisar → iterar.

## Duración: 40 minutos

## Prerequisitos

- Backend API funcional en `localhost:5000` (Módulo 3 completo)
- IDE con asistente de IA (Cursor, Copilot, Kiro o similar)

---

## Paso a Paso

### Paso 1: Crear AGENTS.md (10 min)

Crear en la raíz del repositorio con estas secciones:
- **Stack:** .NET 10, EF Core, Redis, SQS, SignalR / React + Vite + TypeScript, Zustand, TanStack Query
- **Convenciones:** respuestas URM (ApiResponse<T>), FluentValidation, feature-based, DI
- **Patrones:** Repository + Unit of Work, máquina de estados del turno, Output Caching
- **Restricciones:** NO usar EF en controllers, NO devolver entidades (usar DTOs), NO lógica en controllers

### Paso 2: Crear CONTEXT.md (5 min)

Crear en la raíz con:
- Entidades: Paciente, Médico, Turno, Especialidad
- Estados: Pendiente → Confirmado → Completado / Cancelado / NoAsistió
- Reglas: no solapamiento, fecha futura, horario 8-20, duración 30/60 min
- Flujos principales: reservar, confirmar, cancelar, consultar disponibilidad

### Paso 3: Resolver feature con IA — Disponibilidad de Médico (20 min)

**Feature:** `GET /api/medicos/{id}/disponibilidad?fecha=YYYY-MM-DD`

1. Cargar archivos de contexto en el asistente de IA
2. Especificar: input (medicoId, fecha), output (slots libres 30 min entre 8-20), lógica (complemento de ocupados)
3. Pedir generación de: servicio + controller endpoint + test
4. Revisar con criterio:
   - ¿Usa Repository correctamente? ¿Devuelve URM? ¿Valida fecha pasada? ¿Maneja médico inexistente?
5. Iterar hasta que funcione correctamente

### Paso 4: Documentar decisiones (5 min)

Anotar brevemente: qué se aceptó, qué se corrigió, por qué.
Regla: **"Si no lo entendés, no lo mergees"**

---

## Entregable Esperado

- Archivo `AGENTS.md` en la raíz del repositorio
- Archivo `CONTEXT.md` en la raíz del repositorio
- Endpoint `GET /api/medicos/{id}/disponibilidad?fecha=` funcionando
- Nota breve de decisiones sobre el output de la IA

## Conexión con Clase 14

En la próxima clase aplicarás el framework RTC (Rol-Tarea-Criterio) para escribir prompts estructurados. Los archivos de contexto creados hoy serán la base para generar el flujo de confirmación de turno end-to-end con criterios de aceptación predefinidos.
