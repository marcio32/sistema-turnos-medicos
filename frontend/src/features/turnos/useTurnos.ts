import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  getTurnos,
  getTurnoById,
  crearTurno,
  actualizarTurno,
  cancelarTurno,
  type TurnoFilter,
  type CrearTurnoRequest,
  type ActualizarTurnoRequest,
} from './turnosService';

// ─── Query Keys ────────────────────────────────────────────────────────────────

export const turnosKeys = {
  all: ['turnos'] as const,
  lists: () => [...turnosKeys.all, 'list'] as const,
  list: (filter?: TurnoFilter) => [...turnosKeys.lists(), filter] as const,
  details: () => [...turnosKeys.all, 'detail'] as const,
  detail: (id: number) => [...turnosKeys.details(), id] as const,
};

// ─── Queries ───────────────────────────────────────────────────────────────────

/**
 * Hook para listar turnos con filtros opcionales.
 * Soporta paginación, filtro por médico, paciente, fecha y estado.
 */
export function useTurnos(filter?: TurnoFilter) {
  return useQuery({
    queryKey: turnosKeys.list(filter),
    queryFn: () => getTurnos(filter),
  });
}

/**
 * Hook para obtener un turno específico por ID.
 */
export function useTurno(id: number) {
  return useQuery({
    queryKey: turnosKeys.detail(id),
    queryFn: () => getTurnoById(id),
    enabled: id > 0,
  });
}

// ─── Mutations ─────────────────────────────────────────────────────────────────

/**
 * Hook para crear un nuevo turno.
 * Invalida automáticamente la lista de turnos al completarse.
 */
export function useCrearTurno() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CrearTurnoRequest) => crearTurno(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: turnosKeys.all });
    },
  });
}

/**
 * Hook para actualizar un turno existente.
 * Invalida la lista y el detalle del turno actualizado.
 */
export function useActualizarTurno() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: ActualizarTurnoRequest }) =>
      actualizarTurno(id, data),
    onSuccess: (_response, { id }) => {
      queryClient.invalidateQueries({ queryKey: turnosKeys.all });
      queryClient.invalidateQueries({ queryKey: turnosKeys.detail(id) });
    },
  });
}

/**
 * Hook para cancelar un turno.
 * Invalida automáticamente la lista de turnos al completarse.
 */
export function useCancelarTurno() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => cancelarTurno(id),
    onSuccess: (_response, id) => {
      queryClient.invalidateQueries({ queryKey: turnosKeys.all });
      queryClient.invalidateQueries({ queryKey: turnosKeys.detail(id) });
    },
  });
}
