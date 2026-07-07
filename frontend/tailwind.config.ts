import type { Config } from 'tailwindcss';

export default {
  content: [
    './index.html',
    './src/**/*.{js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        turno: {
          pendiente: {
            light: '#fef3c7',
            DEFAULT: '#f59e0b',
            dark: '#b45309',
          },
          confirmado: {
            light: '#d1fae5',
            DEFAULT: '#10b981',
            dark: '#065f46',
          },
          cancelado: {
            light: '#fee2e2',
            DEFAULT: '#ef4444',
            dark: '#991b1b',
          },
          completado: {
            light: '#dbeafe',
            DEFAULT: '#3b82f6',
            dark: '#1e40af',
          },
          noAsistio: {
            light: '#f3f4f6',
            DEFAULT: '#6b7280',
            dark: '#374151',
          },
        },
      },
    },
  },
  plugins: [],
} satisfies Config;
