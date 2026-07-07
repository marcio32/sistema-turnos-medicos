import { apiClient } from '@services/api';
import type { Turno } from './types';

// ─── Unified Response Model (URM) Types ────────────────────────────────────────

export interface StatusInfo {
  code: number;
  message: string;
}

export interface ErrorDetail {
  field: string;
  message: string;
  code: string;
}

export interface PaginationInfo {
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface MetadataInfo {
  timestamp: string;
  pagination?: PaginationInfo;
}

export interface ApiResponse<T> {
  data: T;
  status: StatusInfo;
  errors: ErrorDetail[];
  metadata: MetadataInfo;
}

// ─── Request / Filter Types ────────────────────────────────────────────────────

export interface TurnoFilter {
  pacienteId?: number;
  medicoId?: number;
  fecha?: string;
  estado?: string;
  page?: number;
  pageSize?: number;
}

export interface CrearTurnoRequest {
  pacienteId: number;
  medicoId: number;
  fecha: string;
  hora: string;
  duracion: 30 | 60;
  motivo: string;
}

export interface ActualizarTurnoRequest {
  pacienteId: number;
  medicoId: number;
  fecha: string;
  hora: string;
  duracion: 30 | 60;
  estado: string;
  motivo: string;
}

// ─── Service Functions ─────────────────────────────────────────────────────────

/**
 * Lista turnos con filtros opcionales y paginación.
 */
export function getTurnos(filter?: TurnoFilter): Promise<ApiResponse<Turno[]>> {
  const params: Record<string, string> = {};

  if (filter?.pacienteId) params.pacienteId = String(filter.pacienteId);
  if (filter?.medicoId) params.medicoId = String(filter.medicoId);
  if (filter?.fecha) params.fecha = filter.fecha;
  if (filter?.estado) params.estado = filter.estado;
  if (filter?.page) params.page = String(filter.page);
  if (filter?.pageSize) params.pageSize = String(filter.pageSize);

  return apiClient.get<ApiResponse<Turno[]>>('/turnos', { params });
}

/**
 * Obtiene un turno por su ID.
 */
export function getTurnoById(id: number): Promise<ApiResponse<Turno>> {
  return apiClient.get<ApiResponse<Turno>>(`/turnos/${id}`);
}

/**
 * Crea un nuevo turno.
 */
export function crearTurno(data: CrearTurnoRequest): Promise<ApiResponse<Turno>> {
  return apiClient.post<ApiResponse<Turno>>('/turnos', data);
}

/**
 * Actualiza un turno existente.
 */
export function actualizarTurno(
  id: number,
  data: ActualizarTurnoRequest,
): Promise<ApiResponse<Turno>> {
  return apiClient.put<ApiResponse<Turno>>(`/turnos/${id}`, data);
}

/**
 * Cancela un turno (cambia su estado a "cancelado").
 */
export function cancelarTurno(id: number): Promise<ApiResponse<Turno>> {
  return apiClient.delete<ApiResponse<Turno>>(`/turnos/${id}`);
}
