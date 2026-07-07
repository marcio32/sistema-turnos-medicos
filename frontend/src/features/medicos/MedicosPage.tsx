import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { getMedicos, getDisponibilidad, medicosKeys } from './medicosService';
import type { Medico } from './medicosService';
import { Card, Spinner, ErrorMessage, Button } from '@shared/components';

export default function MedicosPage() {
  const { data, isLoading, error } = useQuery({
    queryKey: medicosKeys.lists(),
    queryFn: getMedicos,
  });

  const medicos: Medico[] = data?.data ?? [];

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-800">Médicos</h1>
      <p className="mt-1 text-gray-600">
        Directorio de médicos y consulta de disponibilidad.
      </p>

      <div className="mt-6">
        {isLoading && (
          <div className="flex items-center justify-center py-12">
            <Spinner size="lg" />
          </div>
        )}

        {error && <ErrorMessage message={error.message} />}

        {!isLoading && !error && medicos.length === 0 && (
          <p className="py-12 text-center text-gray-500">No hay médicos registrados.</p>
        )}

        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {medicos.map((medico) => (
            <MedicoCard key={medico.id} medico={medico} />
          ))}
        </div>
      </div>
    </div>
  );
}

function MedicoCard({ medico }: { medico: Medico }) {
  const [fecha, setFecha] = useState('');
  const [showDisponibilidad, setShowDisponibilidad] = useState(false);

  const { data: disponibilidad, isLoading } = useQuery({
    queryKey: medicosKeys.disponibilidad(medico.id, fecha),
    queryFn: () => getDisponibilidad(medico.id, fecha),
    enabled: showDisponibilidad && fecha.length > 0,
  });

  const slots = disponibilidad?.data?.slotsDisponibles ?? [];

  return (
    <Card>
      <div className="flex items-start justify-between">
        <div>
          <h3 className="font-semibold text-gray-800">{medico.nombre}</h3>
          <p className="text-sm text-gray-500">Matrícula: {medico.matricula}</p>
        </div>
        <span className="rounded-full bg-blue-50 px-2.5 py-0.5 text-xs font-medium text-blue-700">
          {medico.especialidadNombre || 'General'}
        </span>
      </div>

      <div className="mt-4 border-t pt-3">
        <label className="text-xs font-medium text-gray-600">Consultar disponibilidad:</label>
        <div className="mt-1 flex gap-2">
          <input
            type="date"
            value={fecha}
            onChange={(e) => setFecha(e.target.value)}
            className="rounded border border-gray-300 px-2 py-1 text-sm"
          />
          <Button
            size="sm"
            variant="secondary"
            onClick={() => setShowDisponibilidad(true)}
            disabled={!fecha}
          >
            Ver
          </Button>
        </div>

        {showDisponibilidad && fecha && (
          <div className="mt-3">
            {isLoading ? (
              <Spinner size="sm" />
            ) : slots.length > 0 ? (
              <div className="flex flex-wrap gap-1.5">
                {slots.map((slot) => (
                  <span
                    key={slot}
                    className="rounded bg-green-50 px-2 py-0.5 text-xs font-medium text-green-700"
                  >
                    {slot}
                  </span>
                ))}
              </div>
            ) : (
              <p className="text-xs text-gray-500">Sin disponibilidad para esta fecha.</p>
            )}
          </div>
        )}
      </div>
    </Card>
  );
}
