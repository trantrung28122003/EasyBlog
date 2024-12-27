import SockJS from 'sockjs-client';
import { Client } from '@stomp/stompjs';
import { BASE_URL } from '../constants/API';

const SOCKET_URL = BASE_URL + "/ws";
let client: Client | null = null;


export const getWebSocketClient = (): Client => {
    if (!client) {
        client = new Client({
            webSocketFactory: () => {
                const token = localStorage.getItem('token');
                const socket = new SockJS(SOCKET_URL);
                socket.onopen = () => {
                    socket.send(JSON.stringify({
                        headers: {
                            'Authorization': `Bearer ${token}`,
                        },
                    }));
                    
                };

                return socket;
            },
            reconnectDelay: 5000,
            debug: (str) => console.log("[WebSocket Debug]:", str),
        });
    }
    return client;
};
