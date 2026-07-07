/** Estados posibles de un turno médico */
export type EstadoTurno =
  | 'pendiente'
  | 'confirmado'
  | 'cancelado'
  | 'completado'
  | 'noAsistio';

/** Modelo de dominio de un turno médico */
export interface Turno {
  id: number;
  pacienteId: number;
  pacienteNombre: string;
  medicoId: number;
  medicoNombre: string;
  fecha: string;       // formato YYYY-MM-DD
  hora: string;        // formato HH:mm
  duracion: 30 | 60;
  estado: EstadoTurno;
  motivo: string;
  createdAt: string;   // ISO datetime
}
