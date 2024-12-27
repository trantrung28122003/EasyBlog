import { createContext, useContext, useEffect, ReactNode } from "react";
import { Client } from "@stomp/stompjs";
import { getWebSocketClient } from "../hooks/websocket";

const WebSocketContext = createContext<Client | null>(null);

export const useWebSocket = () => {
  return useContext(WebSocketContext);
};

interface WebSocketProviderProps {
  children: ReactNode;
}

export const WebSocketProvider = ({ children }: WebSocketProviderProps) => {
  const client = getWebSocketClient();
  return (
    <WebSocketContext.Provider value={client}>
      {children}
    </WebSocketContext.Provider>
  );
};
