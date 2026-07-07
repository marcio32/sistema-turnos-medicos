import { apiClient } from '@services/api';
import type { ApiResponse } from '../turnos/turnosService';

// ─── Types ─────────────────────────────────────────────────────────────────────

export interface Medico {
  id: number;
  nombre: string;
  matricula: string;
  especialidadId: number;
  especialidadNombre: string;
}

export interface DisponibilidadResponse {
  medicoId: number;
  medicoNombre: string;
  fecha: string;
  slotsDisponibles: string[];   // formato HH:mm (TimeOnly serializado)
}

// ─── Query Keys ────────────────────────────────────────────────────────────────

export const medicosKeys = {
  all: ['medicos'] as const,
  lists: () => [...medicosKeys.all, 'list'] as const,
  details: () => [...medicosKeys.all, 'detail'] as const,
  detail: (id: number) => [...medicosKeys.details(), id] as const,
  disponibilidad: (medicoId: number, fecha: string) =>
    [...medicosKeys.all, medicoId, 'disponibilidad', fecha] as const,
};

// ─── Service Functions ─────────────────────────────────────────────────────────

/**
 * Lista todos los médicos disponibles.
 */
export function getMedicos(): Promise<ApiResponse<Medico[]>> {
  return apiClient.get<ApiResponse<Medico[]>>('/medicos');
}

/**
 * Consulta la disponibilidad de un médico para una fecha determinada.
 * Retorna los slots horarios libres dentro del rango laboral (8:00-20:00).
 */
export function getDisponibilidad(
  medicoId: number,
  fecha: string,
): Promise<ApiResponse<DisponibilidadResponse>> {
  return apiClient.get<ApiResponse<DisponibilidadResponse>>(
    `/medicos/${medicoId}/disponibilidad`,
    { params: { fecha } },
  );
}
