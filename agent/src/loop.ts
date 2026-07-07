import Anthropic from "@anthropic-ai/sdk";
import { tools, executeTool } from "./tools.js";
import { config } from "./config.js";

/** Message type for the conversation history */
type Message = Anthropic.MessageParam;

/** Anthropic client instance */
const anthropic = new Anthropic({ apiKey: config.apiKey });

/**
 * Ejecuta el loop agéntico completo.
 *
 * Ciclo: enviar mensaje → detectar stop_reason → ejecutar tool_use →
 * devolver tool_result → iterar hasta respuesta final o límite de pasos.
 *
 * @param userMessage - Mensaje del usuario en lenguaje natural
 * @param maxSteps - Máximo de iteraciones del loop (default: 5)
 * @returns Respuesta final del agente en texto
 */
export async function runAgent(
  userMessage: string,
  maxSteps: number = config.maxSteps
): Promise<string> {
  const messages: Message[] = [{ role: "user", content: userMessage }];
  let steps = 0;

  while (steps < maxSteps) {
    steps++;
    console.log(`[Paso ${steps}] Llamando al modelo...`);

    const response = await anthropic.messages.create({
      model: config.model,
      max_tokens: 1024,
      tools: tools as Anthropic.Tool[],
      messages,
    });

    // Si el stop_reason no es "tool_use", el modelo terminó con texto final
    if (response.stop_reason !== "tool_use") {
      const textBlock = response.content.find(
        (b): b is Anthropic.TextBlock => b.type === "text"
      );
      return textBlock?.text ?? "Sin respuesta";
    }

    // Procesar tool_use: extraer nombre e input
    const toolUse = response.content.find(
      (b): b is Anthropic.ToolUseBlock => b.type === "tool_use"
    );

    if (!toolUse) {
      return "Error: stop_reason fue tool_use pero no se encontró bloque tool_use";
    }

    console.log(
      `[Paso ${steps}] Tool: ${toolUse.name}, Input: ${JSON.stringify(toolUse.input)}`
    );

    // Ejecutar la tool contra el backend
    const result = await executeTool(
      toolUse.name,
      toolUse.input as Record<string, unknown>
    );
    const resultSummary =
      JSON.stringify(result).length > 200
        ? JSON.stringify(result).substring(0, 200) + "..."
        : JSON.stringify(result);
    console.log(`[Paso ${steps}] Resultado: ${resultSummary}`);

    // Agregar respuesta del asistente y tool_result al historial
    messages.push({ role: "assistant", content: response.content });
    messages.push({
      role: "user",
      content: [
        {
          type: "tool_result",
          tool_use_id: toolUse.id,
          content: JSON.stringify(result),
        },
      ],
    });
  }

  return "Se alcanzó el límite de pasos. Por favor, intentá con una consulta más específica.";
}
