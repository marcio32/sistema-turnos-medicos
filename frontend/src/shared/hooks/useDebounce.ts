import { useEffect, useState } from 'react';

/**
 * Hook que retorna un valor debounced luego del delay especificado.
 * @param value - Valor a debouncear
 * @param delay - Delay en milisegundos (default: 300ms)
 * @returns Valor debounced
 */
export function useDebounce<T>(value: T, delay = 300): T {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(timer);
    };
  }, [value, delay]);

  return debouncedValue;
}
