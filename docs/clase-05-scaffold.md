# Clase 5 — Scaffold React con Arquitectura Feature-Based

## Objetivo

Crear desde cero la estructura del frontend del Sistema de Turnos Médicos con Vite, React, TypeScript y arquitectura feature-based. Al finalizar tendrás un proyecto corriendo en localhost con routing, Tailwind y tu primer componente renderizando datos.

## Duración estimada: 60 minutos

## Prerrequisitos

- Node.js 18+ instalado
- Repositorio del curso clonado (Módulo 1 completado)
- VS Code con extensiones: ES7 React Snippets, Tailwind CSS IntelliSense

---

## Pasos

### Paso 1: Setup con Vite (10 min)

```bash
npm create vite@latest frontend -- --template react-ts
cd frontend
npm install
npm install react-router-dom @tanstack/react-query zustand react-hook-form @hookform/resolvers zod tailwindcss @tailwindcss/vite @microsoft/signalr
```

Verificar que `npm run dev` levanta el proyecto en `http://localhost:5173`.

### Paso 2: Configurar Tailwind CSS (5 min)

En `vite.config.ts` agregar el plugin de Tailwind:

```ts
import tailwindcss from "@tailwindcss/vite";
export default defineConfig({ plugins: [react(), tailwindcss()] });
```

En `src/index.css` reemplazar el contenido por: `@import "tailwindcss";`

### Paso 3: Configurar path aliases (5 min)

En `vite.config.ts` agregar resolve aliases:

```ts
resolve: {
  alias: {
    "@features": "/src/features",
    "@shared": "/src/shared",
  }
}
```

Actualizar `tsconfig.json` con los mismos paths para que TypeScript resuelva los imports.

### Paso 4: Crear estructura feature-based (10 min)

Crear las siguientes carpetas dentro de `src/`:

```
src/
├── features/
│   ├── auth/
│   ├── turnos/
│   ├── medicos/
│   └── admin/
└── shared/
    ├── components/
    ├── hooks/
    ├── stores/
    ├── utils/
    └── services/
```

Crear `src/shared/services/api.ts` con un cliente HTTP base (fetch wrapper con baseURL configurable).

### Paso 5: Configurar React Router v7 (10 min)

Crear `src/router.tsx` con rutas declarativas:

- `/login` → página de login (placeholder)
- `/dashboard` → dashboard principal
- `/turnos` → listado de turnos
- `/medicos` → listado de médicos

Integrar el Router en `src/App.tsx` con `<RouterProvider>`.

### Paso 6: Crear componente TurnoCard (15 min)

Crear `src/features/turnos/types.ts` con la interfaz Turno (id, paciente, médico, fecha, hora, estado, motivo).

Crear `src/features/turnos/TurnoCard.tsx` que renderice:
- Nombre del médico y especialidad
- Fecha y hora del turno
- Estado con un badge de color (pendiente=amarillo, confirmado=verde, cancelado=rojo)
- Motivo de consulta

Renderizar 3 turnos dummy en la ruta `/turnos` para verificar que funciona.

### Paso 7: Verificación final (5 min)

- [ ] `npm run dev` levanta sin errores en localhost:5173
- [ ] Las rutas `/login`, `/dashboard`, `/turnos`, `/medicos` responden
- [ ] TurnoCard muestra datos dummy con estilos Tailwind
- [ ] Path aliases `@features` y `@shared` resuelven correctamente

---

## Entregable

Proyecto React funcional en localhost:5173 con estructura feature-based, TurnoCard renderizando datos dummy con Tailwind, routing básico operativo y path aliases configurados.

## Conexión con Clase 6

La estructura creada hoy es la base para construir componentes reutilizables (Button, Card, Modal, Badge) y configurar el estado global con Zustand en la próxima clase.
