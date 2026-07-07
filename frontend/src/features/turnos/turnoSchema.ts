import { z } from 'zod';

/**
 * Schema de validación Zod para el formulario de creación de turnos.
 *
 * Reglas de negocio:
 * - pacienteId: número positivo requerido
 * - medicoId: número positivo requerido
 * - fecha: string, debe ser una fecha futura
 * - hora: string, hora entre 8:00 y 19:xx (para que el turno termine antes de las 20:00)
 * - duracion: exactamente 30 o 60 minutos
 * - motivo: entre 3 y 500 caracteres
 */
export const turnoSchema = z.object({
  pacienteId: z
    .number({ required_error: 'Seleccioná un paciente' })
    .positive('Seleccioná un paciente'),

  medicoId: z
    .number({ required_error: 'Seleccioná un médico' })
    .positive('Seleccioná un médico'),

  fecha: z
    .string({ required_error: 'La fecha es requerida' })
    .min(1, 'La fecha es requerida')
    .refine(
      (val) => {
        const selected = new Date(val);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        return selected > today;
      },
      { message: 'La fecha debe ser futura' }
    ),

  hora: z
    .string({ required_error: 'La hora es requerida' })
    .min(1, 'La hora es requerida')
    .refine(
      (val) => {
        const hour = parseInt(val.split(':')[0], 10);
        return hour >= 8 && hour <= 19;
      },
      { message: 'El horario debe ser entre 8:00 y 20:00' }
    ),

  duracion: z.union([z.literal(30), z.literal(60)], {
    required_error: 'Seleccioná la duración',
    invalid_type_error: 'La duración debe ser 30 o 60 minutos',
  }),

  motivo: z
    .string({ required_error: 'El motivo es requerido' })
    .min(3, 'El motivo debe tener al menos 3 caracteres')
    .max(500, 'El motivo no puede superar los 500 caracteres'),
});

/** Tipo inferido del schema para uso en formularios */
export type TurnoFormData = z.infer<typeof turnoSchema>;
