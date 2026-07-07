# Clase 8 — Testing, Performance y Conexión SignalR

## Objetivo

Escribir tests del frontend con Vitest + Testing Library, aplicar lazy loading por rutas para mejorar performance, y preparar el hook de conexión SignalR para actualizaciones en tiempo real. Al finalizar tendrás un frontend mantenible y listo para producción.

## Duración estimada: 50 minutos

## Prerrequisitos

- Clase 7 completada (CRUD funcional con TanStack Query)
- Instalar dependencias de testing: `npm install -D vitest @testing-library/react @testing-library/jest-dom jsdom`

---

## Pasos

### Paso 1: Configurar Vitest (5 min)

En `vite.config.ts` agregar bloque `test: { environment: "jsdom", globals: true, setupFiles: "./tests/setup.ts" }`.

Crear `tests/setup.ts` con import de `@testing-library/jest-dom`.

Agregar script en `package.json`: `"test": "vitest run"`

### Paso 2: Test de TurnoCard (10 min)

Crear `tests/components/TurnoCard.test.tsx`:
- Test: renderiza nombre del médico y horario correctamente
- Test: muestra badge con color correcto según estado (confirmado→verde)

Patrón: Arrange (datos mock) → Act (render) → Assert (verificar output con `screen.getByText`).

### Paso 3: Test del formulario TurnoForm (10 min)

Crear `tests/forms/TurnoForm.test.tsx`:
- Test: renderiza todos los campos del formulario
- Test: muestra errores de validación al enviar vacío
- Test: llama a onSubmit con datos válidos

Usar `userEvent` para simular interacciones (click, type, select).

### Paso 4: Test del hook useTurnos (8 min)

Crear `tests/hooks/useTurnos.test.ts`:
- Envolver en `QueryClientProvider` para testing
- Test: retorna loading inicialmente
- Test: retorna datos después de fetch exitoso

Usar `renderHook` de Testing Library y `waitFor` para estados async.

### Paso 5: Refactorizar con separación de responsabilidades (5 min)

Elegir una feature (ej: TurnosList) y separar:
- Lógica de datos → hook custom `useTurnosQuery`
- Lógica de UI → componente presentacional puro
- Lógica de estado → store Zustand

Esto mejora testabilidad: podés testear cada capa por separado.

### Paso 6: Lazy loading por rutas (5 min)

En `src/router.tsx` usar `React.lazy()` para importar cada página (Dashboard, Turnos, Médicos, Admin) y envolverlas en `<Suspense fallback={<Spinner />}>`.

Verificar en DevTools → Network que cada ruta carga un chunk separado.

### Paso 7: Hook useSignalR (7 min)

Crear `src/shared/hooks/useSignalR.ts` que:
- Construya conexión con `HubConnectionBuilder` y reconexión automática
- Se suscriba a eventos: TurnoCreado, TurnoCancelado, TurnoActualizado
- Actualice el store de Zustand cuando llegue un evento
- Desconecte al desmontar el componente (cleanup en useEffect)

El hook queda preparado para conectarse al hub `/hubs/turnos` del backend Módulo 3.

---

## Entregable

Frontend con 3+ tests pasando (`npm run test`), code splitting por rutas (verificable en Network tab), separación de responsabilidades aplicada, y hook useSignalR preparado para conectarse al backend.

## Conexión con Clase 9

El frontend está completo y production-ready. En la Clase 9 comenzamos el backend .NET 10 que expondrá la API real y el hub SignalR al que se conectará este frontend.
