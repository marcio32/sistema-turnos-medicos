import type { Turno } from './types';
import TurnoCard from './TurnoCard';

interface TurnosListProps {
  turnos: Turno[];
  isLoading: boolean;
  error: string | null;
}

export default function TurnosList({ turnos, isLoading, error }: TurnosListProps) {
  // Estado: cargando
  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="h-8 w-8 animate-spin rounded-full border-4 border-gray-200 border-t-blue-600" />
        <span className="ml-3 text-sm text-gray-500">Cargando turnos…</span>
      </div>
    );
  }

  // Estado: error
  if (error) {
    return (
      <div className="rounded-lg border border-red-200 bg-red-50 p-4 text-sm text-red-700">
        <p className="font-medium">Error al cargar turnos</p>
        <p className="mt-1">{error}</p>
      </div>
    );
  }

  // Estado: lista vacía
  if (turnos.length === 0) {
    return (
      <div className="py-12 text-center">
        <svg
          className="mx-auto h-12 w-12 text-gray-300"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          strokeWidth={1.5}
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5"
          />
        </svg>
        <p className="mt-4 text-sm text-gray-500">No hay turnos para mostrar.</p>
      </div>
    );
  }

  // Estado: éxito con datos
  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {turnos.map((turno) => (
        <TurnoCard key={turno.id} turno={turno} />
      ))}
    </div>
  );
}
