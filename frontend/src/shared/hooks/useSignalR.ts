import { useEffect, useRef, useState, useCallback } from 'react';
import {
  HubConnectionBuilder,
  HubConnection,
  HttpTransportType,
  HubConnectionState,
} from '@microsoft/signalr';
import type { Turno } from '../../features/turnos/types';

export interface UseSignalROptions {
  hubUrl?: string;
  onTurnoCreado?: (turno: Turno) => void;
  onTurnoCancelado?: (turno: Turno) => void;
  onTurnoActualizado?: (turno: Turno) => void;
}

export interface UseSignalRReturn {
  connection: HubConnection | null;
  isConnected: boolean;
  error: Error | null;
}

export function useSignalR({
  hubUrl = 'http://localhost:5000/hubs/turnos',
  onTurnoCreado,
  onTurnoCancelado,
  onTurnoActualizado,
}: UseSignalROptions = {}): UseSignalRReturn {
  const [isConnected, setIsConnected] = useState(false);
  const [error, setError] = useState<Error | null>(null);
  const connectionRef = useRef<HubConnection | null>(null);

  const handleReconnecting = useCallback(() => {
    setIsConnected(false);
  }, []);

  const handleReconnected = useCallback(() => {
    setIsConnected(true);
    setError(null);
  }, []);

  const handleClose = useCallback(() => {
    setIsConnected(false);
  }, []);

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl, {
        transport: HttpTransportType.WebSockets | HttpTransportType.LongPolling,
      })
      .withAutomaticReconnect()
      .build();

    connectionRef.current = connection;

    // Register event handlers
    if (onTurnoCreado) {
      connection.on('TurnoCreado', onTurnoCreado);
    }
    if (onTurnoCancelado) {
      connection.on('TurnoCancelado', onTurnoCancelado);
    }
    if (onTurnoActualizado) {
      connection.on('TurnoActualizado', onTurnoActualizado);
    }

    // Connection lifecycle events
    connection.onreconnecting(handleReconnecting);
    connection.onreconnected(handleReconnected);
    connection.onclose(handleClose);

    // Start connection
    connection
      .start()
      .then(() => {
        setIsConnected(true);
        setError(null);
      })
      .catch((err: Error) => {
        setError(err);
        setIsConnected(false);
      });

    // Cleanup on unmount
    return () => {
      if (connection.state !== HubConnectionState.Disconnected) {
        connection.stop();
      }
    };
  }, [hubUrl, onTurnoCreado, onTurnoCancelado, onTurnoActualizado, handleReconnecting, handleReconnected, handleClose]);

  return {
    connection: connectionRef.current,
    isConnected,
    error,
  };
}
