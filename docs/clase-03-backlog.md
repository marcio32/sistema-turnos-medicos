# Clase 3 — Lab Práctico: Organización del Backlog

## Objetivo

Crear el backlog del Sistema de Turnos Médicos en GitHub Projects, escribir historias de usuario con criterios de aceptación para los módulos principales y aplicar priorización MoSCoW.

## Duración: 40 minutos

## Equipos: Los mismos de clases anteriores

---

## Paso a paso

### Parte 1: Crear proyecto en GitHub Projects (5 min)

1. Ir al repositorio → Projects → New project
2. Seleccionar template "Board" (Kanban)
3. Nombre: "Sistema de Turnos Médicos — Sprint Backlog"
4. Crear columnas: `Backlog` | `To Do` | `In Progress` | `Done`
5. Agregar campo custom "Prioridad" con valores: Must, Should, Could, Won't

### Parte 2: Escribir historias de usuario (20 min)

Crear issues en GitHub con este formato y asignarlas al proyecto:

**Módulo Pacientes (2 historias):**

1. Título: `Como paciente, quiero registrarme en el sistema para poder sacar turnos`
   - Criterios de aceptación:
     - Dado un formulario con nombre, email, teléfono y DNI
     - Cuando completo todos los campos y envío
     - Entonces se crea mi perfil y puedo iniciar sesión

2. Título: `Como paciente, quiero ver mis turnos para conocer mis próximas citas`
   - Criterios: lista filtrada por estado, orden cronológico

**Módulo Médicos (2 historias):**

3. Título: `Como médico, quiero ver mi agenda diaria para organizar mis consultas`
4. Título: `Como médico, quiero marcar un turno como completado al finalizar la consulta`

**Módulo Turnos (3 historias):**

5. Título: `Como paciente, quiero crear un turno eligiendo médico, fecha y hora`
   - Criterios: validar que la fecha sea futura, horario entre 8:00 y 20:00, duración 30 o 60 min
6. Título: `Como paciente, quiero cancelar un turno con al menos 2hs de anticipación`
   - Criterios: verificar tiempo de anticipación, cambiar estado a "Cancelado", notificar al médico
7. Título: `Como sistema, quiero validar que no se solapen turnos de un mismo médico`
   - Criterios: rechazar creación si existe turno en rango [hora, hora+duración)

### Parte 3: Priorización MoSCoW (10 min)

1. Clasificar cada historia según impacto:
   - **Must have**: Crear turno, ver agenda, validar solapamiento
   - **Should have**: Cancelar turno, ver mis turnos
   - **Could have**: Registro de paciente (se puede hacer con seed data)
   - **Won't (this sprint)**: Marcar completado (depende del backend completo)
2. Asignar el campo "Prioridad" a cada issue
3. Mover los "Must have" a la columna `To Do`

### Parte 4: Conectar tickets con branches (5 min)

1. Crear labels en el repo:
   - `modulo:pacientes` (color azul)
   - `modulo:medicos` (color verde)
   - `modulo:turnos` (color naranja)
2. Etiquetar cada historia con su label correspondiente
3. Elegir una historia "Must have" (ej: #5 Crear turno)
4. Desde el issue, crear un branch: `feature/#5-crear-turno`
5. Verificar que el issue linkea al branch automáticamente
6. Agregar milestone "Módulo 2 - Frontend" a las historias que correspondan

---

## Entregable

- GitHub Project Board con columnas configuradas
- Al menos 7 historias de usuario con criterios de aceptación
- Priorización MoSCoW aplicada a todas las historias
- Un branch linkeado a un issue con naming convention

## Conexión con la Clase 4

En la próxima clase simularemos un sprint completo usando este backlog: haremos sprint planning seleccionando historias, daily standup, review del trabajo y retrospectiva.
