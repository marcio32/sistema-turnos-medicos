import type { Turno, EstadoTurno } from './types';

/** Mapeo de estado a clases Tailwind del badge */
const badgeStyles: Record<EstadoTurno, string> = {
  pendiente: 'bg-turno-pendiente-light text-turno-pendiente-dark',
  confirmado: 'bg-turno-confirmado-light text-turno-confirmado-dark',
  cancelado: 'bg-turno-cancelado-light text-turno-cancelado-dark',
  completado: 'bg-turno-completado-light text-turno-completado-dark',
  noAsistio: 'bg-turno-noAsistio-light text-turno-noAsistio-dark',
};

/** Etiquetas legibles para cada estado */
const estadoLabel: Record<EstadoTurno, string> = {
  pendiente: 'Pendiente',
  confirmado: 'Confirmado',
  cancelado: 'Cancelado',
  completado: 'Completado',
  noAsistio: 'No asistió',
};

interface TurnoCardProps {
  turno: Turno;
}

export default function TurnoCard({ turno }: TurnoCardProps) {
  return (
    <article className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm transition hover:shadow-md">
      {/* Header: médico + badge de estado */}
      <div className="flex items-start justify-between gap-2">
        <h3 className="text-sm font-semibold text-gray-800">
          {turno.medicoNombre}
        </h3>
        <span
          className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${badgeStyles[turno.estado]}`}
        >
          {estadoLabel[turno.estado]}
        </span>
      </div>

      {/* Fecha, hora y duración */}
      <div className="mt-2 flex items-center gap-3 text-sm text-gray-600">
        <span className="flex items-center gap-1">
          <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          {turno.fecha}
        </span>
        <span className="flex items-center gap-1">
          <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          {turno.hora} ({turno.duracion} min)
        </span>
      </div>

      {/* Motivo */}
      {turno.motivo && (
        <p className="mt-2 text-sm text-gray-500">
          <span className="font-medium text-gray-700">Motivo:</span> {turno.motivo}
        </p>
      )}
    </article>
  );
}
