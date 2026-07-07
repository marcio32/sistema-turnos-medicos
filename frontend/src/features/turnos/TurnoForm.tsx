import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { turnoSchema, type TurnoFormData } from './turnoSchema';
import { Button } from '@shared/components';

export interface TurnoFormProps {
  onSubmit: (data: TurnoFormData) => void;
}

// Opciones dummy para selects (se reemplazarán con datos reales del backend)
const pacientesOptions = [
  { id: 1, nombre: 'Juan Pérez' },
  { id: 2, nombre: 'María García' },
  { id: 3, nombre: 'Carlos López' },
];

const medicosOptions = [
  { id: 1, nombre: 'Dr. Roberto Fernández' },
  { id: 2, nombre: 'Dra. Ana Martínez' },
  { id: 3, nombre: 'Dr. Luis Rodríguez' },
];

export default function TurnoForm({ onSubmit }: TurnoFormProps) {
  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<TurnoFormData>({
    resolver: zodResolver(turnoSchema),
    mode: 'onChange', // validación en tiempo real
    defaultValues: {
      pacienteId: undefined,
      medicoId: undefined,
      fecha: '',
      hora: '',
      duracion: undefined,
      motivo: '',
    },
  });

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="space-y-5 max-w-lg"
      noValidate
    >
      {/* Paciente */}
      <div>
        <label
          htmlFor="pacienteId"
          className="block text-sm font-medium text-gray-700 mb-1"
        >
          Paciente
        </label>
        <select
          id="pacienteId"
          className="w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-1 focus:ring-blue-500"
          defaultValue=""
          onChange={(e) =>
            setValue('pacienteId', Number(e.target.value), {
              shouldValidate: true,
            })
          }
        >
          <option value="" disabled>
            Seleccioná un paciente
          </option>
          {pacientesOptions.map((p) => (
            <option key={p.id} value={p.id}>
              {p.nombre}
            </option>
          ))}
        </select>
        {errors.pacienteId && (
          <p className="mt-1 text-sm text-red-600">{errors.pacienteId.message}</p>
        )}
      </div>

      {/* Médico */}
      <div>
        <label
          htmlFor="medicoId"
          className="block text-sm font-medium text-gray-700 mb-1"
        >
          Médico
        </label>
        <select
          id="medicoId"
          className="w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-1 focus:ring-blue-500"
          defaultValue=""
          onChange={(e) =>
            setValue('medicoId', Number(e.target.value), {
              shouldValidate: true,
            })
          }
        >
          <option value="" disabled>
            Seleccioná un médico
          </option>
          {medicosOptions.map((m) => (
            <option key={m.id} value={m.id}>
              {m.nombre}
            </option>
          ))}
        </select>
        {errors.medicoId && (
          <p className="mt-1 text-sm text-red-600">{errors.medicoId.message}</p>
        )}
      </div>

      {/* Fecha */}
      <div>
        <label
          htmlFor="fecha"
          className="block text-sm font-medium text-gray-700 mb-1"
        >
          Fecha
        </label>
        <input
          type="date"
          id="fecha"
          className="w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-1 focus:ring-blue-500"
          {...register('fecha')}
        />
        {errors.fecha && (
          <p className="mt-1 text-sm text-red-600">{errors.fecha.message}</p>
        )}
      </div>

      {/* Hora */}
      <div>
        <label
          htmlFor="hora"
          className="block text-sm font-medium text-gray-700 mb-1"
        >
          Hora
        </label>
        <input
          type="time"
          id="hora"
          step="1800"
          min="08:00"
          max="19:30"
          className="w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-1 focus:ring-blue-500"
          {...register('hora')}
        />
        {errors.hora && (
          <p className="mt-1 text-sm text-red-600">{errors.hora.message}</p>
        )}
      </div>

      {/* Duración */}
      <fieldset>
        <legend className="block text-sm font-medium text-gray-700 mb-1">
          Duración
        </legend>
        <div className="flex gap-6">
          <label className="inline-flex items-center gap-2 cursor-pointer">
            <input
              type="radio"
              value="30"
              className="text-blue-600 focus:ring-blue-500"
              onChange={() =>
                setValue('duracion', 30, { shouldValidate: true })
              }
            />
            <span className="text-sm text-gray-700">30 minutos</span>
          </label>
          <label className="inline-flex items-center gap-2 cursor-pointer">
            <input
              type="radio"
              value="60"
              className="text-blue-600 focus:ring-blue-500"
              onChange={() =>
                setValue('duracion', 60, { shouldValidate: true })
              }
            />
            <span className="text-sm text-gray-700">60 minutos</span>
          </label>
        </div>
        {errors.duracion && (
          <p className="mt-1 text-sm text-red-600">{errors.duracion.message}</p>
        )}
      </fieldset>

      {/* Motivo */}
      <div>
        <label
          htmlFor="motivo"
          className="block text-sm font-medium text-gray-700 mb-1"
        >
          Motivo de la consulta
        </label>
        <textarea
          id="motivo"
          rows={3}
          className="w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-1 focus:ring-blue-500"
          placeholder="Describí brevemente el motivo de la consulta..."
          {...register('motivo')}
        />
        {errors.motivo && (
          <p className="mt-1 text-sm text-red-600">{errors.motivo.message}</p>
        )}
      </div>

      {/* Submit */}
      <Button type="submit" variant="primary" size="md" disabled={isSubmitting}>
        {isSubmitting ? 'Guardando...' : 'Reservar turno'}
      </Button>
    </form>
  );
}
