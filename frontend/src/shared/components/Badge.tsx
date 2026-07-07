export type EstadoTurno =
  | 'pendiente'
  | 'confirmado'
  | 'cancelado'
  | 'completado'
  | 'noAsistio';

export interface BadgeProps {
  estado: EstadoTurno;
  className?: string;
}

const estadoStyles: Record<EstadoTurno, string> = {
  pendiente: 'bg-turno-pendiente-light text-turno-pendiente-dark',
  confirmado: 'bg-turno-confirmado-light text-turno-confirmado-dark',
  cancelado: 'bg-turno-cancelado-light text-turno-cancelado-dark',
  completado: 'bg-turno-completado-light text-turno-completado-dark',
  noAsistio: 'bg-turno-noAsistio-light text-turno-noAsistio-dark',
};

const estadoLabels: Record<EstadoTurno, string> = {
  pendiente: 'Pendiente',
  confirmado: 'Confirmado',
  cancelado: 'Cancelado',
  completado: 'Completado',
  noAsistio: 'No Asistió',
};

export function Badge({ estado, className = '' }: BadgeProps) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${estadoStyles[estado]} ${className}`}
    >
      {estadoLabels[estado]}
    </span>
  );
}
