import { useQuery } from '@tanstack/react-query';
import { getTurnos } from '@features/turnos/turnosService';
import { getMedicos } from '@features/medicos/medicosService';
import { turnosKeys } from '@features/turnos/useTurnos';
import { medicosKeys } from '@features/medicos/medicosService';
import { Card, Spinner } from '@shared/components';
import type { Turno } from '@features/turnos/types';

export default function DashboardPage() {
  const { data: turnosData, isLoading: loadingTurnos } = useQuery({
    queryKey: turnosKeys.lists(),
    queryFn: () => getTurnos(),
  });

  const { data: medicosData, isLoading: loadingMedicos } = useQuery({
    queryKey: medicosKeys.lists(),
    queryFn: getMedicos,
  });

  const turnos: Turno[] = turnosData?.data ?? [];
  const totalMedicos = medicosData?.data?.length ?? 0;

  const pendientes = turnos.filter((t) => t.estado === 'pendiente').length;
  const confirmados = turnos.filter((t) => t.estado === 'confirmado').length;
  const completados = turnos.filter((t) => t.estado === 'completado').length;
  const cancelados = turnos.filter((t) => t.estado === 'cancelado').length;

  const isLoading = loadingTurnos || loadingMedicos;

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-800">Dashboard</h1>
      <p className="mt-1 text-gray-600">
        Panel principal del Sistema de Gestión de Turnos Médicos.
      </p>

      {isLoading ? (
        <div className="mt-8 flex justify-center">
          <Spinner size="lg" />
        </div>
      ) : (
        <>
          <div className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <StatCard label="Turnos Pendientes" value={pendientes} color="text-amber-600" bg="bg-amber-50" />
            <StatCard label="Turnos Confirmados" value={confirmados} color="text-green-600" bg="bg-green-50" />
            <StatCard label="Turnos Completados" value={completados} color="text-blue-600" bg="bg-blue-50" />
            <StatCard label="Médicos Activos" value={totalMedicos} color="text-purple-600" bg="bg-purple-50" />
          </div>

          <div className="mt-6 grid gap-4 lg:grid-cols-2">
            <Card title="Resumen de Estados">
              <div className="space-y-2">
                <Bar label="Pendientes" value={pendientes} total={turnos.length} color="bg-amber-400" />
                <Bar label="Confirmados" value={confirmados} total={turnos.length} color="bg-green-400" />
                <Bar label="Completados" value={completados} total={turnos.length} color="bg-blue-400" />
                <Bar label="Cancelados" value={cancelados} total={turnos.length} color="bg-red-400" />
              </div>
            </Card>

            <Card title="Últimos Turnos">
              {turnos.length === 0 ? (
                <p className="text-sm text-gray-500">No hay turnos registrados.</p>
              ) : (
                <ul className="space-y-2">
                  {turnos.slice(0, 5).map((t) => (
                    <li key={t.id} className="flex items-center justify-between text-sm">
                      <span className="text-gray-700">
                        {t.pacienteNombre} → {t.medicoNombre}
                      </span>
                      <span className="text-xs text-gray-500">{t.fecha} {t.hora}</span>
                    </li>
                  ))}
                </ul>
              )}
            </Card>
          </div>
        </>
      )}
    </div>
  );
}

function StatCard({ label, value, color, bg }: { label: string; value: number; color: string; bg: string }) {
  return (
    <div className={`rounded-lg border p-4 ${bg}`}>
      <p className="text-sm font-medium text-gray-600">{label}</p>
      <p className={`mt-1 text-3xl font-bold ${color}`}>{value}</p>
    </div>
  );
}

function Bar({ label, value, total, color }: { label: string; value: number; total: number; color: string }) {
  const pct = total > 0 ? (value / total) * 100 : 0;
  return (
    <div>
      <div className="flex justify-between text-xs text-gray-600">
        <span>{label}</span>
        <span>{value}</span>
      </div>
      <div className="mt-1 h-2 rounded-full bg-gray-100">
        <div className={`h-2 rounded-full ${color}`} style={{ width: `${pct}%` }} />
      </div>
    </div>
  );
}
