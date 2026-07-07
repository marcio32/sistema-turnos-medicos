import * as readline from "node:readline/promises";
import { stdin as input, stdout as output } from "node:process";
import { runAgent } from "./loop.js";
import { config } from "./config.js";

/**
 * Entry point del agente de turnos médicos.
 *
 * Inicia un REPL interactivo donde el usuario puede escribir consultas
 * en lenguaje natural y el agente las resuelve usando tool use contra
 * el backend del Sistema de Turnos.
 *
 * Flujo de prueba sugerido:
 *   "Quiero sacar un turno con el Dr. García para mañana a las 10"
 *
 * Consideraciones de producción:
 * - Verificación de API key antes de iniciar
 * - Límite de tokens por ejecución (max_tokens en config)
 * - Manejo de errores con fallback amigable
 * - Permisos mínimos: tools de lectura sin confirmación
 */

const WELCOME_MESSAGE = `
╔══════════════════════════════════════════════════════════════╗
║        🏥 Agente de Turnos Médicos — Tool Use               ║
╠══════════════════════════════════════════════════════════════╣
║  Escribí tu consulta en lenguaje natural y el agente        ║
║  interactúa con el backend para resolver tu pedido.         ║
║                                                             ║
║  Ejemplos:                                                  ║
║  • "Quiero sacar un turno con el Dr. García para            ║
║     mañana a las 10"                                        ║
║  • "Mostrame los turnos del paciente 1"                     ║
║  • "¿Qué disponibilidad tiene el Dr. García el lunes?"      ║
║  • "Cancelar el turno 42"                                   ║
║                                                             ║
║  Escribí "salir" o presioná CTRL+C para terminar.           ║
╚══════════════════════════════════════════════════════════════╝
`;

async function main(): Promise<void> {
  // --- Consideración de producción: verificar API key ---
  if (!config.apiKey) {
    console.error(
      "❌ Error: No se encontró la API key de Anthropic.\n" +
        "   Configurá la variable de entorno ANTHROPIC_API_KEY en un archivo .env\n" +
        "   o exportala en tu terminal:\n\n" +
        "   export ANTHROPIC_API_KEY=sk-ant-...\n"
    );
    process.exit(1);
  }

  console.log(WELCOME_MESSAGE);

  const rl = readline.createInterface({ input, output });

  // --- Consideración de producción: manejo graceful de CTRL+C ---
  rl.on("close", () => {
    console.log("\n👋 ¡Hasta luego! Cerrando el agente...");
    process.exit(0);
  });

  // REPL loop
  while (true) {
    const userInput = await rl.question("🧑 Vos: ");

    const trimmed = userInput.trim();

    if (!trimmed) {
      continue;
    }

    if (trimmed.toLowerCase() === "salir" || trimmed.toLowerCase() === "exit") {
      console.log("👋 ¡Hasta luego!");
      rl.close();
      break;
    }

    try {
      console.log("\n🤖 Procesando...\n");

      // --- Consideración de producción: límite de pasos (tokens indirecto) ---
      // El maxSteps en config limita las iteraciones del loop agéntico,
      // controlando el consumo de tokens por ejecución.
      const response = await runAgent(trimmed, config.maxSteps);

      console.log(`🤖 Agente: ${response}\n`);
    } catch (error: unknown) {
      // --- Consideración de producción: fallback amigable ---
      if (error instanceof Error) {
        if (error.message.includes("401") || error.message.includes("authentication")) {
          console.error(
            "❌ Error de autenticación. Verificá que tu ANTHROPIC_API_KEY sea válida.\n"
          );
        } else if (error.message.includes("ECONNREFUSED") || error.message.includes("fetch")) {
          console.error(
            "❌ No se pudo conectar con el backend. Asegurate de que la API esté corriendo en " +
              config.baseUrl +
              "\n"
          );
        } else {
          console.error(
            `❌ Ocurrió un error inesperado: ${error.message}\n` +
              "   Intentá de nuevo o reformulá tu consulta.\n"
          );
        }
      } else {
        console.error("❌ Error desconocido. Intentá de nuevo.\n");
      }
    }
  }
}

// Ejecutar el entry point
main();
