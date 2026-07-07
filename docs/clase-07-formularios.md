# Clase 7 — Formularios, APIs y Estados Asíncronos con TanStack Query

## Objetivo

Construir el formulario de creación de turnos con React Hook Form + Zod, conectar el frontend a una API mock y manejar estados asíncronos con TanStack Query para lograr un CRUD completo funcional.

## Duración estimada: 50 minutos

## Prerrequisitos

- Clase 6 completada (componentes Button, Card, Modal, Badge + store Zustand)
- Instalar dependencia de API mock: `npm install -D json-server`

---

## Pasos

### Paso 1: Configurar API mock con json-server (5 min)

Crear `frontend/db.json` con turnos y médicos de ejemplo (2-3 registros cada uno).

Agregar script en `package.json`: `"mock-api": "json-server --watch db.json --port 3001"`

Levantar con `npm run mock-api` en otra terminal.

### Paso 2: Schema de validación con Zod (8 min)

Crear `src/features/turnos/turnoSchema.ts` con reglas:
- `pacienteId`: number positivo
- `medicoId`: number positivo
- `fecha`: string con refine → debe ser futura
- `hora`: string con refine → entre 8:00 y 20:00
- `duracion`: literal 30 o 60
- `motivo`: string min 3, max 500 caracteres

### Paso 3: Formulario con React Hook Form (12 min)

Crear `src/features/turnos/TurnoForm.tsx`:
- Usar `useForm` con `zodResolver(turnoSchema)`
- Campos: select paciente, select médico, input date, select hora (slots 8:00-19:30), radio duración (30/60), textarea motivo
- Mostrar errores de validación en tiempo real debajo de cada campo
- Botón submit usando el componente Button (variant="primary")
- Usar el componente Card como contenedor del formulario

### Paso 4: Configurar TanStack Query (5 min)

En `src/App.tsx` envolver la app con `QueryClientProvider` (staleTime: 5min, retry: 1).

Crear `src/features/turnos/turnosService.ts` con funciones fetch:
- `listarTurnos()` → GET /turnos
- `crearTurno(data)` → POST /turnos
- `actualizarTurno(id, data)` → PUT /turnos/:id
- `cancelarTurno(id)` → PATCH /turnos/:id con estado "cancelado"

### Paso 5: Hooks con useQuery y useMutation (10 min)

Crear `src/features/turnos/useTurnos.ts` con:
- `useTurnos()` → useQuery con queryKey ["turnos"]
- `useCrearTurno()` → useMutation + invalidateQueries en onSuccess
- `useCancelarTurno()` → useMutation + invalidateQueries

Integrar en TurnosList: Spinner en loading, ErrorMessage en error, mensaje en lista vacía.

### Paso 6: Conectar formulario con mutation (5 min)

En TurnoForm, al hacer submit:
1. Llamar `useCrearTurno().mutate(data)`
2. Mostrar Spinner durante la creación
3. En éxito: cerrar formulario (Modal), mostrar feedback
4. En error: mostrar ErrorMessage

### Paso 7: Implementar cancelación (5 min)

Agregar botón "Cancelar" en TurnoCard que use `useCancelarTurno` mutation. Confirmar con Modal antes de ejecutar. Invalidar cache automáticamente.

---

## Entregable

CRUD completo de turnos contra API mock: listar, crear, editar y cancelar turnos, con validación Zod en tiempo real, feedback visual de estados (loading/success/error) y cache automático via TanStack Query.

## Conexión con Clase 8

El CRUD funcional de hoy será la base para escribir tests con Vitest, aplicar lazy loading por rutas y preparar la conexión real-time con SignalR del backend.
