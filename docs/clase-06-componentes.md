# Clase 6 — Componentes Reutilizables y Estado con Zustand

## Objetivo

Construir una librería de componentes reutilizables (Button, Card, Modal, Badge) y configurar estado global del módulo de turnos con Zustand. Al finalizar tendrás componentes profesionales y un store funcional.

## Duración estimada: 40 minutos

## Prerrequisitos

- Clase 5 completada (scaffold con estructura feature-based y Tailwind)
- Proyecto corriendo en localhost:5173

---

## Pasos

### Paso 1: Componente Button (8 min)

Crear `src/shared/components/Button.tsx` con:
- Props: `variant` (primary | secondary | danger), `size` (sm | md | lg), `children`, `onClick`, `disabled`
- Estilos Tailwind por variante: primary=azul, secondary=gris, danger=rojo
- Tamaños: sm=px-2 py-1 text-sm, md=px-4 py-2, lg=px-6 py-3 text-lg

```tsx
type ButtonProps = {
  variant?: "primary" | "secondary" | "danger";
  size?: "sm" | "md" | "lg";
  children: React.ReactNode;
  onClick?: () => void;
  disabled?: boolean;
};
```

### Paso 2: Componente Card (5 min)

Crear `src/shared/components/Card.tsx` con composition pattern:
- Props: `children`, `className` (opcional)
- Estilos base: borde redondeado, sombra suave, padding
- Usar `children` para permitir contenido flexible

### Paso 3: Componente Modal (10 min)

Crear `src/shared/components/Modal.tsx` con:
- Props: `isOpen`, `onClose`, `title`, `children`
- Renderizar con `createPortal` al body
- Overlay oscuro con click para cerrar
- Botón X para cerrar

Crear `src/shared/hooks/useModal.ts` con hook que retorne `{ isOpen, open, close, toggle }`.

### Paso 4: Componente Badge (5 min)

Crear `src/shared/components/Badge.tsx` para estados de turno:
- Props: `estado` (pendiente | confirmado | cancelado | completado)
- Colores: pendiente=amarillo, confirmado=verde, cancelado=rojo, completado=azul
- Texto capitalizado del estado

Integrar Badge dentro de TurnoCard reemplazando el texto plano del estado.

### Paso 5: Barrel export (2 min)

Crear `src/shared/components/index.ts` exportando todos los componentes:

```ts
export { Button } from "./Button";
export { Card } from "./Card";
export { Modal } from "./Modal";
export { Badge } from "./Badge";
```

### Paso 6: Store Zustand para Turnos (8 min)

Crear `src/shared/stores/useTurnosStore.ts`:

```ts
interface TurnosState {
  turnos: Turno[];
  agregarTurno: (turno: Turno) => void;
  eliminarTurno: (id: number) => void;
  actualizarEstado: (id: number, estado: EstadoTurno) => void;
}
```

Inicializar con 3-4 turnos dummy. Conectar el store a `TurnosList` para que renderice desde Zustand.

Crear `src/shared/stores/useAuthStore.ts` con estado básico: `{ user, token, login, logout }`.

### Paso 7: Custom hooks utilitarios (2 min)

Crear en `src/shared/hooks/`:
- `useToggle.ts` — alterna boolean (útil para modales y dropdowns)
- `useLocalStorage.ts` — persiste estado en localStorage
- `useDebounce.ts` — retrasa valor (útil para búsquedas)

---

## Entregable

Librería de componentes (Button, Card, Modal, Badge) integrada en la app, con un store de Zustand funcional que gestiona la lista de turnos en estado local. TurnoCard usa Badge, la lista se renderiza desde el store.

## Conexión con Clase 7

Los componentes y el store creados hoy se usarán para construir el formulario de creación de turnos con React Hook Form + Zod y conectar a una API mock con TanStack Query.
