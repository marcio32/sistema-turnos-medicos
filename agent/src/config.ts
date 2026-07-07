import "dotenv/config";

export const config = {
  /** Anthropic API key for authentication */
  apiKey: process.env.ANTHROPIC_API_KEY ?? "",

  /** Model to use for the agent */
  model: process.env.ANTHROPIC_MODEL ?? "claude-sonnet-4-20250514",

  /** Maximum number of agentic loop iterations */
  maxSteps: parseInt(process.env.MAX_STEPS ?? "5", 10),

  /** Base URL of the backend API */
  baseUrl: process.env.API_BASE_URL ?? "http://localhost:5000/api",
} as const;
