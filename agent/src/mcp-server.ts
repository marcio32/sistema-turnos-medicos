/**
 * MCP Server para el Sistema de Gestión de Turnos Médicos.
 *
 * Expone las 5 tools del sistema como un MCP server usando transporte stdin/stdout,
 * permitiendo que cualquier cliente MCP-compatible (Kiro, Claude Desktop, etc.)
 * se conecte al backend de turnos médicos.
 *
 * Uso:
 *   npx tsx ./agent/src/mcp-server.ts
 *
 * El servidor escucha mensajes JSON-RPC por stdin y responde por stdout
 * siguiendo el Model Context Protocol (MCP).
 */

import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { z } from "zod";
import { executeTool } from "./tools.js";

// ---------- Crear instancia del MCP Server ----------

const server = new McpServer({
  name: "turnos-medicos",
  version: "1.0.0",
  description:
    "MCP Server del Sistema de Gestión de Turnos Médicos — expone consulta de disponibilidad, reserva, cancelación, listado y recordatorios.",
});

// ---------- Registrar Tools ----------

server.tool(
  "consultar_disponibilidad",
  "Consulta los horarios libres de un médico en una fecha determinada",
  {
    medicoId: z.number().describe("ID del médico"),
    fecha: z.string().describe("Fecha en formato YYYY-MM-DD"),
  },
  async ({ medicoId, fecha }) => {
    const result = await executeTool("consultar_disponibilidad", { medicoId, fecha });
    return {
      content: [{ type: "text" as const, text: JSON.stringify(result, null, 2) }],
    };
  }
);

server.tool(
  "reservar_turno",
  "Crea un nuevo turno médico para un paciente con un médico en fecha y hora específica",
  {
    pacienteId: z.number().describe("ID del paciente"),
    medicoId: z.number().describe("ID del médico"),
    fecha: z.string().describe("Fecha en formato YYYY-MM-DD"),
    hora: z.string().describe("Hora en formato HH:mm"),
    duracion: z.number().describe("Duración en minutos (30 o 60)"),
    motivo: z.string().optional().describe("Motivo de la consulta"),
  },
  async ({ pacienteId, medicoId, fecha, hora, duracion, motivo }) => {
    const input: Record<string, unknown> = { pacienteId, medicoId, fecha, hora, duracion };
    if (motivo) input.motivo = motivo;
    const result = await executeTool("reservar_turno", input);
    return {
      content: [{ type: "text" as const, text: JSON.stringify(result, null, 2) }],
    };
  }
);

server.tool(
  "cancelar_turno",
  "Cancela un turno existente por su ID",
  {
    turnoId: z.number().describe("ID del turno a cancelar"),
  },
  async ({ turnoId }) => {
    const result = await executeTool("cancelar_turno", { turnoId });
    return {
      content: [{ type: "text" as const, text: JSON.stringify(result, null, 2) }],
    };
  }
);

server.tool(
  "listar_turnos_paciente",
  "Lista todos los turnos de un paciente",
  {
    pacienteId: z.number().describe("ID del paciente"),
  },
  async ({ pacienteId }) => {
    const result = await executeTool("listar_turnos_paciente", { pacienteId });
    return {
      content: [{ type: "text" as const, text: JSON.stringify(result, null, 2) }],
    };
  }
);

server.tool(
  "enviar_recordatorio",
  "Envía un recordatorio del turno via cola SQS al paciente",
  {
    turnoId: z.number().describe("ID del turno"),
    mensaje: z.string().optional().describe("Mensaje personalizado del recordatorio"),
  },
  async ({ turnoId, mensaje }) => {
    const input: Record<string, unknown> = { turnoId };
    if (mensaje) input.mensaje = mensaje;
    const result = await executeTool("enviar_recordatorio", input);
    return {
      content: [{ type: "text" as const, text: JSON.stringify(result, null, 2) }],
    };
  }
);

// ---------- Iniciar transporte stdin/stdout ----------

async function main() {
  const transport = new StdioServerTransport();
  await server.connect(transport);
  // El servidor queda escuchando por stdin hasta que el cliente cierre la conexión.
}

main().catch((error) => {
  console.error("Error fatal al iniciar MCP server:", error);
  process.exit(1);
});
