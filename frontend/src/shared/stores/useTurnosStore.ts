import { create } from 'zustand';
import type { EstadoTurno, Turno } from '@features/turnos/types';

interface TurnosState {
  turnos: Turno[];
  isLoading: boolean;
  error: string | null;
}

interface TurnosActions {
  setTurnos: (turnos: Turno[]) => void;
  addTurno: (turno: Turno) => void;
  removeTurno: (id: number) => void;
  updateTurnoEstado: (id: number, newEstado: EstadoTurno) => void;
  setLoading: (isLoading: boolean) => void;
  setError: (error: string | null) => void;
}

export type TurnosStore = TurnosState & TurnosActions;

export const useTurnosStore = create<TurnosStore>((set) => ({
  // State
  turnos: [],
  isLoading: false,
  error: null,

  // Actions
  setTurnos: (turnos) => set({ turnos }),

  addTurno: (turno) =>
    set((state) => ({ turnos: [...state.turnos, turno] })),

  removeTurno: (id) =>
    set((state) => ({
      turnos: state.turnos.filter((t) => t.id !== id),
    })),

  updateTurnoEstado: (id, newEstado) =>
    set((state) => ({
      turnos: state.turnos.map((t) =>
        t.id === id ? { ...t, estado: newEstado } : t,
      ),
    })),

  setLoading: (isLoading) => set({ isLoading }),

  setError: (error) => set({ error }),
}));
