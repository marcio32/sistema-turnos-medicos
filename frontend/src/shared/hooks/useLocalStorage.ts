import { useCallback, useState } from 'react';

/**
 * Hook genérico que sincroniza estado con localStorage.
 * @param key - Clave de localStorage
 * @param initialValue - Valor inicial si no existe en localStorage
 * @returns [value, setValue]
 */
export function useLocalStorage<T>(key: string, initialValue: T): [T, (value: T | ((prev: T) => T)) => void] {
  const [storedValue, setStoredValue] = useState<T>(() => {
    try {
      const item = window.localStorage.getItem(key);
      return item ? (JSON.parse(item) as T) : initialValue;
    } catch {
      return initialValue;
    }
  });

  const setValue = useCallback(
    (value: T | ((prev: T) => T)) => {
      setStoredValue((prev) => {
        const valueToStore = value instanceof Function ? value(prev) : value;
        try {
          window.localStorage.setItem(key, JSON.stringify(valueToStore));
        } catch {
          // Si localStorage está lleno o no disponible, seguimos con el state en memoria
        }
        return valueToStore;
      });
    },
    [key],
  );

  return [storedValue, setValue];
}
