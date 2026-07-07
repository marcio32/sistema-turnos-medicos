# Clase 4 — Challenge Práctico: Sprint Simulado

## Objetivo

Simular un sprint completo del Sistema de Turnos Médicos: Sprint Planning con Sprint Goal, daily standup simulado, sprint review del trabajo del módulo y retrospectiva con formato Start/Stop/Continue.

## Duración: 40 minutos

## Equipos: Los mismos de clases anteriores

---

## Paso a paso

### Parte 1: Sprint Planning (12 min)

1. **Definir Sprint Goal** (2 min):
   - Escribir en `docs/sprint-planning.md`:
   > Sprint Goal: "Al finalizar este sprint, un paciente puede crear y cancelar turnos con validación de solapamiento"

2. **Seleccionar historias del backlog** (5 min):
   - Del Project Board, mover a `To Do` las historias Must/Should que aportan al goal
   - Estimar con Planning Poker simplificado (S/M/L):
     - Crear turno → M
     - Validar solapamiento → M
     - Cancelar turno → S
     - Ver agenda médico → L
   - Comprometer las que entren en la capacidad (S + M + M = sprint de 1 semana)

3. **Descomponer en tareas** (5 min):
   - Para "Crear turno", crear sub-tareas como issues:
     - `task: Crear TurnoForm con validación Zod`
     - `task: Crear endpoint POST /api/turnos`
     - `task: Conectar form con API via TanStack Query`
   - Asignar cada tarea a un integrante

### Parte 2: Daily Standup simulado (8 min)

1. El Scrum Master (rotativo) abre la reunión y cronometra
2. Cada integrante responde (máximo 2 min por persona):
   - **¿Qué hice?** → "Configuré ESLint y CI en el repo"
   - **¿Qué voy a hacer?** → "Empiezo con el TurnoForm"
   - **¿Tengo bloqueos?** → "Necesito definir los campos del formulario"
3. El Scrum Master anota bloqueos visibles para todos
4. Proponer solución inmediata o agendar reunión aparte
5. Mover las tareas en el board: `To Do` → `In Progress`
6. Total del daily: máximo 5 minutos (simular la restricción real)

### Parte 3: Sprint Review (10 min)

1. Cada equipo presenta en 3 minutos lo logrado en el Módulo 1:
   - Mostrar el repo con branches y PRs mergeadas
   - Mostrar el CI corriendo en verde
   - Mostrar el Project Board con historias priorizadas
2. Los demás equipos hacen una pregunta o sugerencia
3. Registrar feedback en un comentario del issue de Sprint

### Parte 4: Retrospectiva — Start/Stop/Continue (10 min)

1. Abrir `docs/retrospectiva.md` y completar:

   | Start (empezar a hacer) | Stop (dejar de hacer) | Continue (seguir haciendo) |
   |---|---|---|
   | Revisar PRs más rápido | Commitear directo a main | Usar conventional commits |
   | Descomponer tareas antes de codificar | Issues sin criterios de aceptación | Code review con checklist |

2. Votar las top 2 acciones (cada uno pone ✓ en sus favoritas)
3. Convertir las acciones ganadoras en issues del proyecto:
   - `improvement: Establecer SLA de 24hs para code review`
   - `improvement: Template obligatorio para issues`

---

## Entregable

- Sprint Planning documentado en `docs/sprint-planning.md` con Sprint Goal y historias comprometidas
- Tareas descompuestas como issues con asignaciones
- Daily simulado completado (evidencia en board actualizado con tareas en "In Progress")
- Sprint Review presentado al grupo con feedback recibido
- Retrospectiva en `docs/retrospectiva.md` con al menos 6 items y 2 acciones concretas convertidas en issues

## Conexión con la Clase 5

Con el equipo organizado, el backlog priorizado y el proceso ágil establecido, la próxima clase arrancamos a codificar: scaffold del frontend React con Vite y arquitectura feature-based.
