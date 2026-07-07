import { useState } from 'react';
import TurnosList from './TurnosList';
import TurnoForm from './TurnoForm';
import { useTurnos, useCrearTurno } from './useTurnos';
import { Button, Modal } from '@shared/components';
import type { TurnoFormData } from './turnoSchema';
import type { Turno } from './types';

export default function TurnosPage() {
  const [showForm, setShowForm] = useState(false);
  const { data, isLoading, error } = useTurnos();
  const crearTurno = useCrearTurno();

  // Mapear la respuesta URM a la lista de turnos
  const turnos: Turno[] = data?.data ?? [];
  const errorMessage = error?.message ?? (data?.errors?.length ? data.errors[0].message : null);

  function handleSubmit(formData: TurnoFormData) {
    crearTurno.mutate(
      {
        pacienteId: formData.pacienteId,
        medicoId: formData.medicoId,
        fecha: formData.fecha,
        hora: formData.hora,
        duracion: formData.duracion,
        motivo: formData.motivo,
      },
      {
        onSuccess: () => setShowForm(false),
      }
    );
  }

  return (
    <div>
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-800">Turnos</h1>
          <p className="mt-1 text-gray-600">
            Gestión de turnos médicos — listado y administración.
          </p>
        </div>
        <Button variant="primary" onClick={() => setShowForm(true)}>
          + Nuevo Turno
        </Button>
      </div>

      <div className="mt-6">
        <TurnosList
          turnos={turnos}
          isLoading={isLoading}
          error={errorMessage}
        />
      </div>

      <Modal isOpen={showForm} onClose={() => setShowForm(false)} title="Nuevo Turno">
        <TurnoForm onSubmit={handleSubmit} />
        {crearTurno.error && (
          <p className="mt-3 text-sm text-red-600">
            Error al crear turno: {crearTurno.error.message}
          </p>
        )}
      </Modal>
    </div>
  );
}
