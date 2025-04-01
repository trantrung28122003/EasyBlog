import { useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";

export const useSignalR = (hubUrl: string) => {
    const connectionRef = useRef<signalR.HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);

    useEffect(() => {
        if (!connectionRef.current) {
            const newConnection = new signalR.HubConnectionBuilder()
                .withUrl(hubUrl, {
                    withCredentials: true,
                    transport: signalR.HttpTransportType.WebSockets,
                })
                .withAutomaticReconnect()
                .configureLogging(signalR.LogLevel.Information)
                .build();

            connectionRef.current = newConnection;

            newConnection.start()
                .then(() => {
                    console.log("✅ Kết nối SignalR thành công");
                    setIsConnected(true);
                })
                .catch((err) => console.error("❌ Lỗi kết nối SignalR:", err));
        }

        return () => {
            if (connectionRef.current?.state === signalR.HubConnectionState.Connected) {
                connectionRef.current.stop();
                console.log("🚪 Đóng kết nối SignalR");
            }
        };
    }, [hubUrl]);

    return connectionRef.current;
};
