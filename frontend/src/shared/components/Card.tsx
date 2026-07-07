import type { ReactNode } from 'react';

export interface CardProps {
  children: ReactNode;
  title?: string;
  className?: string;
}

export function Card({ children, title, className = '' }: CardProps) {
  return (
    <div
      className={`rounded-lg border border-gray-200 bg-white p-4 shadow-sm ${className}`}
    >
      {title && (
        <h3 className="mb-3 text-lg font-semibold text-gray-900">{title}</h3>
      )}
      {children}
    </div>
  );
}
