import { useEffect, useRef, useCallback } from "react";
import { createEvent, createStore } from "effector";
import { useUnit } from "effector-react";
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";

export enum WsStatus {
  open,
  closed,
  error,
}

const open = createEvent<string>("open");
const closed = createEvent<Error | undefined>("closed");
const error = createEvent("error");

const wsStatus = createStore<WsStatus>(WsStatus.closed)
  .on(open, () => WsStatus.open)
  .on(closed, () => WsStatus.closed)
  .on(error, () => WsStatus.error);

export function useWebSocket(
  uri: string,
  onConnected?: () => void,
  onError?: (e: Error) => void
) {
  const status = useUnit(wsStatus);
  const connectionRef = useRef<HubConnection>();

  const handleOpen = () => {
    open(connectionRef.current?.connectionId ?? "");
    if (onConnected) onConnected();
  };

  const handleError = (err: Error) => {
    error();
    if (onError) onError(err);
  };

  useEffect(() => {
    if (!uri || uri === "") return;
    const connection = new HubConnectionBuilder()
      .withUrl(uri)
      .configureLogging(LogLevel.Warning)
      .build();

    connectionRef.current = connection;
    connectionRef.current?.onclose(closed);
    connectionRef.current?.onreconnected((c) => {
      console.log("reconnected", c);
      open(c ?? "");
    });

    connectionRef.current.start().then(handleOpen).catch(handleError);

    return () => {
      console.log("stopped");
      connectionRef.current?.stop();
    };
  }, [uri]);

  const subscribe = useCallback(
    <T>(endpoint: string, handler: (msg: T) => void) => {
      if (connectionRef.current?.state === HubConnectionState.Connected) {
        connectionRef.current.on(endpoint, handler);
      }
    },
    [connectionRef]
  );

  const unSubscribe = useCallback(
    (endpoint: string) => {
      if (connectionRef.current?.state === HubConnectionState.Connected)
        connectionRef.current.off(endpoint);
    },
    [connectionRef]
  );

  const sendMessage = useCallback(
    <T>(sendMethod: string, message?: T) => {
      if (connectionRef.current?.state !== HubConnectionState.Connected) {
        console.log("not connected");
        return;
      }

      connectionRef.current?.send(sendMethod, message);
    },
    [connectionRef]
  );
  return { status, sendMessage, subscribe, unSubscribe };
}
