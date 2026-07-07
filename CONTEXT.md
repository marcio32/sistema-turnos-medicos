# CONTEXT.md — Dominio del Sistema de Turnos Médicos

## Descripción del Dominio

Sistema de gestión de turnos médicos que permite a pacientes reservar, confirmar y cancelar turnos con médicos de distintas especialidades. El sistema gestiona la disponibilidad de los profesionales, valida reglas de negocio (solapamiento, horarios, anticipación) y notifica a las partes involucradas en tiempo real.

## Entidades Principales

### Turno

La entidad central del sistema. Representa una cita médica agendada.

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| Id | int | Identificador único |
| PacienteId | int (FK) | Referencia al paciente |
| MedicoId | int (FK) | Referencia al médico |
| Fecha | DateOnly | Fecha del turno |
| Hora | TimeOnly | Hora de inicio |
| Duracion | int | Duración en minutos (30 o 60) |
| Estado | EstadoTurno | Estado actual del turno |
| Motivo | string | Motivo de la consulta |
| CreatedAt | DateTime | Fecha de creación |
| UpdatedAt | DateTime? | Última modificación |

### Medico

Profesional médico que atiende turnos.

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| Id | int | Identificador único |
| Nombre | string | Nombre completo del profesional |
| Matricula | string | Número de matrícula profesional |
| EspecialidadId | int (FK) | Referencia a la especialidad |

### Paciente

Persona que solicita turnos médicos.

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| Id | int | Identificador único |
| Nombre | string | Nombre completo |
| Email | string | Correo electrónico |
| Telefono | string | Número de teléfono |
| DNI | string | Documento Nacional de Identidad |

### Especialidad

Área de especialización médica.

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| Id | int | Identificador único |
| Nombre | string | Nombre de la especialidad |
| Descripcion | string | Descripción de la especialidad |

## Relaciones

- **Especialidad → Médicos**: Una especialidad tiene muchos médicos (1:N)
- **Médico → Turnos**: Un médico tiene muchos turnos (1:N)
- **Paciente → Turnos**: Un paciente tiene muchos turnos (1:N)

## Máquina de Estados del Turno

### Estados Posibles

| Estado | Descripción |
|--------|-------------|
| `Pendiente` | Turno recién creado, esperando confirmación del paciente |
| `Confirmado` | Paciente confirmó asistencia |
| `Cancelado` | Turno cancelado por paciente o médico |
| `Completado` | Turno atendido exitosamente |
| `NoAsistio` | Paciente no se presentó a la cita |

### Transiciones Válidas

```
[Crear] ──────────────────────────► Pendiente
Pendiente ──[Confirmar]──────────► Confirmado
Pendiente ──[Cancelar]───────────► Cancelado
Confirmado ──[Cancelar]──────────► Cancelado
Confirmado ──[Completar]─────────► Completado
Confirmado ──[Marcar ausencia]───► NoAsistio
```

### Condiciones para Transiciones

| Transición | Condición |
|-----------|-----------|
| Pendiente → Confirmado | Dentro de las 24 horas desde la creación |
| Pendiente → Cancelado | Al menos 2 horas de anticipación al horario del turno |
| Confirmado → Cancelado | Al menos 2 horas de anticipación al horario del turno |
| Confirmado → Completado | Solo el médico puede marcar como completado |
| Confirmado → NoAsistio | Posterior al horario del turno |

## Reglas de Negocio

### 1. No solapamiento de turnos

Un médico NO puede tener dos turnos cuyo intervalo `[hora, hora + duración)` se superponga en la misma fecha.

**Ejemplo**: Si el Dr. García tiene un turno a las 10:00 de 30 min, no puede tener otro turno entre 10:00 y 10:29 inclusive.

### 2. Fecha futura obligatoria

Solo se pueden crear turnos para fechas estrictamente futuras. No se permite crear turnos para hoy ni para fechas pasadas.

### 3. Horario laboral

Los turnos solo se pueden agendar entre las **08:00 y las 20:00** en días hábiles (lunes a viernes). El turno completo (hora + duración) debe caber dentro del rango laboral.

**Ejemplo**: Un turno de 60 minutos no puede comenzar a las 19:30 porque terminaría a las 20:30.

### 4. Duración válida

Los turnos solo pueden durar **30 minutos** o **60 minutos**. No se aceptan otras duraciones.

### 5. Confirmación dentro de 24 horas

El paciente debe confirmar el turno dentro de las 24 horas posteriores a la creación. Si no confirma a tiempo, el turno puede ser liberado.

### 6. Cancelación con 2 horas de anticipación

Para cancelar un turno (tanto el paciente como el médico), se requiere al menos **2 horas de anticipación** respecto al horario programado del turno.

## Flujos Principales

### Flujo 1: Crear Turno

1. El paciente selecciona un médico y una fecha
2. El sistema consulta la disponibilidad del médico en esa fecha
3. El paciente elige un horario libre y la duración (30/60 min)
4. El sistema valida:
   - La fecha es futura
   - El horario está dentro del rango laboral (8:00–20:00)
   - No hay solapamiento con turnos existentes del médico
   - La duración es válida (30 o 60)
5. Se crea el turno en estado `Pendiente`
6. Se publica evento `turno.creado` a la cola SQS
7. Se notifica via SignalR a los clientes conectados

### Flujo 2: Confirmar Turno

1. El paciente solicita confirmar un turno `Pendiente`
2. El sistema valida:
   - El turno está en estado `Pendiente`
   - No pasaron más de 24 horas desde la creación
3. El turno pasa a estado `Confirmado`
4. Se publica evento `turno.confirmado` a la cola SQS
5. Se notifica via SignalR

### Flujo 3: Cancelar Turno

1. El paciente o médico solicita cancelar un turno (`Pendiente` o `Confirmado`)
2. El sistema valida:
   - El turno está en estado `Pendiente` o `Confirmado`
   - Faltan al menos 2 horas para el horario del turno
3. El turno pasa a estado `Cancelado`
4. Se publica evento `turno.cancelado` a la cola SQS
5. Se notifica via SignalR

### Flujo 4: Consultar Disponibilidad

1. Se recibe solicitud con `medicoId` y `fecha`
2. El sistema obtiene todos los turnos del médico en esa fecha (no cancelados)
3. Se genera el rango laboral completo: slots de 30 minutos entre 8:00 y 20:00
4. Se eliminan los slots que se superponen con turnos existentes
5. Se retorna la lista de horarios disponibles

**Ejemplo de respuesta**:
```json
{
  "data": {
    "medicoId": 1,
    "fecha": "2025-01-20",
    "slotsDisponibles": ["08:00", "08:30", "09:00", "10:30", "11:00", ...]
  }
}
```

## Eventos del Sistema (SQS)

| Evento | Cuándo se dispara | Payload incluye |
|--------|-------------------|-----------------|
| `turno.creado` | Al crear un turno nuevo | turnoId, pacienteId, medicoId, fecha, hora |
| `turno.confirmado` | Al confirmar un turno pendiente | turnoId, pacienteId, medicoId, fecha, hora |
| `turno.cancelado` | Al cancelar un turno | turnoId, pacienteId, medicoId, fecha, hora, motivo |

El Worker Service consume estos eventos y procesa notificaciones (email, push, log).

## Endpoints Principales de la API

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/turnos` | Listar turnos (con paginación y filtros) |
| GET | `/api/turnos/{id}` | Obtener turno por ID |
| POST | `/api/turnos` | Crear nuevo turno |
| PUT | `/api/turnos/{id}` | Actualizar turno |
| DELETE | `/api/turnos/{id}` | Cancelar turno |
| PUT | `/api/turnos/{id}/confirmar` | Confirmar turno |
| GET | `/api/medicos/{id}/disponibilidad?fecha=YYYY-MM-DD` | Consultar disponibilidad |
| GET | `/health` | Health check |
