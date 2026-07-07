import React, { Suspense } from 'react';
import { createBrowserRouter, Outlet, NavLink } from 'react-router-dom';
import { Spinner } from '@shared/components';

// Lazy-loaded pages for code splitting
const DashboardPage = React.lazy(() => import('@features/admin/DashboardPage'));
const LoginPage = React.lazy(() => import('@features/auth/LoginPage'));
const TurnosPage = React.lazy(() => import('@features/turnos/TurnosPage'));
const MedicosPage = React.lazy(() => import('@features/medicos/MedicosPage'));

function SuspenseFallback() {
  return (
    <div className="flex items-center justify-center min-h-screen">
      <Spinner size="lg" />
    </div>
  );
}

const navItems = [
  { to: '/', label: 'Dashboard', icon: '🏠' },
  { to: '/turnos', label: 'Turnos', icon: '📅' },
  { to: '/medicos', label: 'Médicos', icon: '👨‍⚕️' },
  { to: '/login', label: 'Login', icon: '🔑' },
];

function Navbar() {
  return (
    <nav className="bg-white border-b border-gray-200 shadow-sm">
      <div className="mx-auto max-w-7xl px-4">
        <div className="flex h-14 items-center justify-between">
          <span className="text-lg font-bold text-blue-600">🏥 Turnos Médicos</span>
          <div className="flex gap-1">
            {navItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                end={item.to === '/'}
                className={({ isActive }) =>
                  `flex items-center gap-1.5 rounded-md px-3 py-2 text-sm font-medium transition-colors ${
                    isActive
                      ? 'bg-blue-50 text-blue-700'
                      : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'
                  }`
                }
              >
                <span>{item.icon}</span>
                <span>{item.label}</span>
              </NavLink>
            ))}
          </div>
        </div>
      </div>
    </nav>
  );
}

function Layout() {
  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <main className="mx-auto max-w-7xl px-4 py-6">
        <Suspense fallback={<SuspenseFallback />}>
          <Outlet />
        </Suspense>
      </main>
    </div>
  );
}

export const router = createBrowserRouter([
  {
    element: <Layout />,
    children: [
      {
        path: '/',
        element: <DashboardPage />,
      },
      {
        path: '/login',
        element: <LoginPage />,
      },
      {
        path: '/turnos',
        element: <TurnosPage />,
      },
      {
        path: '/medicos',
        element: <MedicosPage />,
      },
    ],
  },
]);
