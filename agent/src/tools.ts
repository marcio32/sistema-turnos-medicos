import { config } from "./config";

/**
 * Tool definition following the Anthropic tool use schema.
 */
export interface Tool {
  name: string;
  description: string;
  input_schema: {
    type: "object";
    properties: Record<string, { type: string; description?: string; enum?: number[] }>;
    required: string[];
  };
}

/**
 * Definición de las 5 tools que exponen los endpoints del Backend como
 * herramientas para el agente Anthropic.
 */
export const tools: Tool[] = [
  {
    name: "consultar_disponibilidad",
    description: "Consulta los horarios libres de un médico en una fecha",
    input_schema: {
      type: "object",
      properties: {
        medicoId: { type: "number", description: "ID del médico" },
        fecha: { type: "string", description: "Fecha en formato YYYY-MM-DD" },
      },
      required: ["medicoId", "fecha"],
    },
  },
  {
    name: "reservar_turno",
    description: "Crea un nuevo turno médico",
    input_schema: {
      type: "object",
      properties: {
        pacienteId: { type: "number", description: "ID del paciente" },
        medicoId: { type: "number", description: "ID del médico" },
        fecha: { type: "string", description: "Fecha en formato YYYY-MM-DD" },
        hora: { type: "string", description: "Hora en formato HH:mm" },
        duracion: { type: "number", description: "Duración en minutos", enum: [30, 60] },
        motivo: { type: "string", description: "Motivo de la consulta" },
      },
      required: ["pacienteId", "medicoId", "fecha", "hora", "duracion"],
    },
  },
  {
    name: "cancelar_turno",
    description: "Cancela un turno existente",
    input_schema: {
      type: "object",
      properties: {
        turnoId: { type: "number", description: "ID del turno a cancelar" },
      },
      required: ["turnoId"],
    },
  },
  {
    name: "listar_turnos_paciente",
    description: "Lista todos los turnos de un paciente",
    input_schema: {
      type: "object",
      properties: {
        pacienteId: { type: "number", description: "ID del paciente" },
      },
      required: ["pacienteId"],
    },
  },
  {
    name: "enviar_recordatorio",
    description: "Envía un recordatorio del turno via cola SQS",
    input_schema: {
      type: "object",
      properties: {
        turnoId: { type: "number", description: "ID del turno" },
        mensaje: { type: "string", description: "Mensaje personalizado del recordatorio" },
      },
      required: ["turnoId"],
    },
  },
];

/**
 * Ejecuta una tool llamando al endpoint correspondiente del Backend API.
 * Retorna el JSON parseado de la respuesta, o un objeto de error si el backend
 * no está disponible o la tool no existe.
 *
 * Mapping de tools → endpoints del backend:
 * ─────────────────────────────────────────────────────────────────────
 * | Tool                      | Método | Endpoint                              |
 * |---------------------------|--------|---------------------------------------|
 * | consultar_disponibilidad  | GET    | /api/medicos/{id}/disponibilidad?fecha= |
 * | reservar_turno            | POST   | /api/turnos                           |
 * | cancelar_turno            | DELETE | /api/turnos/{id}                      |
 * | listar_turnos_paciente    | GET    | /api/turnos?pacienteId={id}           |
 * | enviar_recordatorio       | POST   | /api/turnos/{id}/recordatorio         |
 * ─────────────────────────────────────────────────────────────────────
 *
 * Las respuestas siguen el formato URM (Unified Response Model):
 * { data: T, status: { code, message }, errors: [], metadata: { timestamp } }
 */
export async function executeTool(
  name: string,
  input: Record<string, unknown>
): Promise<unknown> {
  try {
    switch (name) {
      case "consultar_disponibilidad": {
        const { medicoId, fecha } = input as { medicoId: number; fecha: string };
        const res = await fetch(
          `${config.baseUrl}/medicos/${medicoId}/disponibilidad?fecha=${fecha}`
        );
        return await res.json();
      }

      case "reservar_turno": {
        const res = await fetch(`${config.baseUrl}/turnos`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(input),
        });
        return await res.json();
      }

      case "cancelar_turno": {
        const { turnoId } = input as { turnoId: number };
        const res = await fetch(`${config.baseUrl}/turnos/${turnoId}`, {
          method: "DELETE",
        });
        // DELETE puede devolver 204 sin body
        if (res.status === 204) {
          return { success: true, message: "Turno cancelado exitosamente" };
        }
        return await res.json();
      }

      case "listar_turnos_paciente": {
        const { pacienteId } = input as { pacienteId: number };
        const res = await fetch(
          `${config.baseUrl}/turnos?pacienteId=${pacienteId}`
        );
        return await res.json();
      }

      case "enviar_recordatorio": {
        const { turnoId, mensaje } = input as { turnoId: number; mensaje?: string };
        const res = await fetch(`${config.baseUrl}/turnos/${turnoId}/recordatorio`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ turnoId, mensaje: mensaje ?? "" }),
        });
        return await res.json();
      }

      default:
        return { error: `Tool desconocida: ${name}` };
    }
  } catch (error) {
    const message =
      error instanceof Error ? error.message : "Error desconocido";
    return {
      error: `No se pudo conectar al backend: ${message}`,
    };
  }
}
