# Product Backlog — Sistema de Gestión de Turnos Médicos

## Priorización MoSCoW

| Prioridad | Descripción |
|-----------|-------------|
| **Must Have** | Funcionalidad esencial sin la cual el sistema no puede operar |
| **Should Have** | Importante pero no crítica para el primer release |
| **Could Have** | Deseable, se incluye si hay tiempo |
| **Won't Have** | Fuera del alcance de este sprint/release |

---

## Módulo: Pacientes

### US-PAC-001: Registrar paciente

**Como** paciente,  
**quiero** registrarme en el sistema con mis datos personales,  
**para** poder solicitar turnos médicos.

**Prioridad:** Must Have  
**Estimación:** 5 puntos  

**Criterios de Aceptación:**
- [ ] DADO que un paciente ingresa nombre, email, teléfono y DNI válidos, CUANDO confirma el registro, ENTONCES el sistema crea el paciente y muestra confirmación
- [ ] DADO que un paciente ingresa un DNI ya registrado, CUANDO intenta registrarse, ENTONCES el sistema muestra un error indicando que el DNI ya existe
- [ ] DADO que un paciente omite campos obligatorios, CUANDO intenta enviar el formulario, ENTONCES el sistema muestra mensajes de validación por campo

---

### US-PAC-002: Consultar mis turnos

**Como** paciente,  
**quiero** ver la lista de mis turnos (pendientes, confirmados, pasados),  
**para** tener visibilidad de mi agenda médica.

**Prioridad:** Must Have  
**Estimación:** 3 puntos  

**Criterios de Aceptación:**
- [ ] DADO que un paciente accede a "Mis Turnos", CUANDO la página carga, ENTONCES se muestran los turnos ordenados por fecha (próximos primero)
- [ ] DADO que un paciente tiene turnos en distintos estados, CUANDO visualiza la lista, ENTONCES cada turno muestra un badge con su estado (Pendiente, Confirmado, Cancelado, Completado)
- [ ] DADO que un paciente no tiene turnos, CUANDO accede a la sección, ENTONCES se muestra un estado vacío con opción de solicitar turno

---

### US-PAC-003: Cancelar turno

**Como** paciente,  
**quiero** cancelar un turno con al menos 2 horas de anticipación,  
**para** liberar el horario para otros pacientes.

**Prioridad:** Must Have  
**Estimación:** 3 puntos  

**Criterios de Aceptación:**
- [ ] DADO que un paciente tiene un turno confirmado o pendiente con más de 2 horas de anticipación, CUANDO solicita cancelar, ENTONCES el sistema cambia el estado a "Cancelado" y notifica al médico
- [ ] DADO que un paciente intenta cancelar con menos de 2 horas de anticipación, CUANDO confirma la acción, ENTONCES el sistema rechaza la cancelación con un mensaje explicativo
- [ ] DADO que un turno se cancela exitosamente, CUANDO se completa la operación, ENTONCES se publica un evento a la cola SQS de notificaciones

---

## Módulo: Médicos

### US-MED-001: Consultar agenda del día

**Como** médico,  
**quiero** ver todos mis turnos del día actual,  
**para** organizarme y preparar las consultas.

**Prioridad:** Must Have  
**Estimación:** 3 puntos  

**Criterios de Aceptación:**
- [ ] DADO que un médico accede a su agenda, CUANDO selecciona una fecha, ENTONCES se muestran todos los turnos de ese día ordenados por hora
- [ ] DADO que un médico tiene turnos, CUANDO visualiza la agenda, ENTONCES cada turno muestra: hora, duración, paciente, motivo y estado
- [ ] DADO que un médico no tiene turnos en la fecha seleccionada, CUANDO consulta la agenda, ENTONCES se muestra un mensaje indicando que no hay turnos programados

---

### US-MED-002: Consultar disponibilidad

**Como** médico (o sistema),  
**quiero** consultar los horarios libres de un médico en una fecha dada,  
**para** ofrecer slots disponibles al paciente al momento de reservar.

**Prioridad:** Must Have  
**Estimación:** 5 puntos  

**Criterios de Aceptación:**
- [ ] DADO que se consulta la disponibilidad de un médico para una fecha, CUANDO se ejecuta la consulta, ENTONCES se retornan los slots libres de 30 minutos entre 8:00 y 20:00
- [ ] DADO que un médico tiene turnos ocupados, CUANDO se calcula disponibilidad, ENTONCES los slots ocupados NO aparecen en la lista de disponibles
- [ ] DADO que se consulta una fecha que es fin de semana, CUANDO se ejecuta la consulta, ENTONCES se retorna una lista vacía (no hay horarios laborales)

---

### US-MED-003: Marcar turno como completado

**Como** médico,  
**quiero** marcar un turno como "Completado" una vez que atendí al paciente,  
**para** mantener actualizado el historial de atenciones.

**Prioridad:** Should Have  
**Estimación:** 2 puntos  

**Criterios de Aceptación:**
- [ ] DADO que un turno está en estado "Confirmado", CUANDO el médico lo marca como completado, ENTONCES el estado cambia a "Completado" y se registra la fecha/hora
- [ ] DADO que un turno está en estado "Pendiente" o "Cancelado", CUANDO el médico intenta marcarlo como completado, ENTONCES el sistema rechaza la acción con un error de transición inválida
- [ ] DADO que un turno se completa, CUANDO se confirma la operación, ENTONCES se notifica via SignalR a los clientes conectados

---

## Módulo: Turnos

### US-TUR-001: Solicitar nuevo turno

**Como** paciente,  
**quiero** solicitar un turno con un médico en un horario disponible,  
**para** recibir atención médica en la fecha y hora que me convenga.

**Prioridad:** Must Have  
**Estimación:** 8 puntos  

**Criterios de Aceptación:**
- [ ] DADO que un paciente selecciona médico, fecha, hora disponible, duración y motivo válidos, CUANDO confirma la solicitud, ENTONCES se crea un turno en estado "Pendiente"
- [ ] DADO que un paciente intenta reservar un horario ya ocupado por otro turno del mismo médico, CUANDO envía la solicitud, ENTONCES el sistema rechaza con error 409 (conflicto de solapamiento)
- [ ] DADO que un paciente selecciona una fecha pasada, CUANDO intenta crear el turno, ENTONCES el sistema rechaza la solicitud con error de validación
- [ ] DADO que un paciente selecciona un horario fuera del rango laboral (antes de 8:00 o después de 20:00), CUANDO envía la solicitud, ENTONCES el sistema rechaza con error de validación
- [ ] DADO que el turno se crea exitosamente, CUANDO se completa la operación, ENTONCES se publica un evento "turno.creado" a la cola SQS

---

### US-TUR-002: Confirmar turno

**Como** paciente,  
**quiero** confirmar mi turno dentro de las primeras 24 horas desde su creación,  
**para** asegurar mi lugar en la agenda del médico.

**Prioridad:** Must Have  
**Estimación:** 3 puntos  

**Criterios de Aceptación:**
- [ ] DADO que un turno está en estado "Pendiente" y han pasado menos de 24 horas desde su creación, CUANDO el paciente confirma, ENTONCES el estado cambia a "Confirmado"
- [ ] DADO que un turno está en estado "Pendiente" y han pasado más de 24 horas, CUANDO el paciente intenta confirmar, ENTONCES el sistema rechaza la confirmación por timeout
- [ ] DADO que un turno se confirma exitosamente, CUANDO se completa la operación, ENTONCES se publica un evento "turno.confirmado" a SQS y se notifica via SignalR

---

### US-TUR-003: Listar turnos con filtros y paginación

**Como** administrador,  
**quiero** listar todos los turnos del sistema con filtros por estado, médico, fecha y paginación,  
**para** gestionar y monitorear la operación.

**Prioridad:** Should Have  
**Estimación:** 5 puntos  

**Criterios de Aceptación:**
- [ ] DADO que un administrador accede al listado de turnos, CUANDO la página carga, ENTONCES se muestran los turnos paginados (10 por página por defecto) con metadata de paginación
- [ ] DADO que un administrador aplica filtros (por estado, por médico, por rango de fechas), CUANDO ejecuta la búsqueda, ENTONCES solo se muestran los turnos que coinciden con los criterios
- [ ] DADO que la respuesta incluye muchos turnos, CUANDO se retorna la lista, ENTONCES el formato URM incluye data, status, errors y metadata con información de paginación

---

### US-TUR-004: Recibir notificaciones en tiempo real

**Como** usuario del sistema (paciente o médico),  
**quiero** recibir notificaciones en tiempo real cuando un turno se crea, confirma o cancela,  
**para** estar informado sin necesidad de refrescar la página.

**Prioridad:** Could Have  
**Estimación:** 5 puntos  

**Criterios de Aceptación:**
- [ ] DADO que un usuario está conectado al hub de SignalR, CUANDO otro usuario crea un turno relevante, ENTONCES recibe una notificación en tiempo real
- [ ] DADO que un turno cambia de estado, CUANDO se completa la operación, ENTONCES todos los clientes suscritos reciben el evento con los datos actualizados
- [ ] DADO que la conexión SignalR se pierde, CUANDO el cliente detecta la desconexión, ENTONCES intenta reconectarse automáticamente

---

## Resumen de Priorización

| ID | Historia | Prioridad | Puntos |
|----|----------|-----------|--------|
| US-PAC-001 | Registrar paciente | Must Have | 5 |
| US-PAC-002 | Consultar mis turnos | Must Have | 3 |
| US-PAC-003 | Cancelar turno | Must Have | 3 |
| US-MED-001 | Consultar agenda del día | Must Have | 3 |
| US-MED-002 | Consultar disponibilidad | Must Have | 5 |
| US-MED-003 | Marcar turno como completado | Should Have | 2 |
| US-TUR-001 | Solicitar nuevo turno | Must Have | 8 |
| US-TUR-002 | Confirmar turno | Must Have | 3 |
| US-TUR-003 | Listar turnos con filtros | Should Have | 5 |
| US-TUR-004 | Notificaciones en tiempo real | Could Have | 5 |
| **Total** | | | **42** |
