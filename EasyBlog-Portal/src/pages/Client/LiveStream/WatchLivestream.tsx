import React, { useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import { getWebSocketClient } from "../../../hooks/websocket";

const WatchLivestream: React.FC = () => {
  const { streamId } = useParams<{ streamId: string }>();
  const remoteVideoRef = useRef<HTMLVideoElement>(null);
  const client = getWebSocketClient();

  useEffect(() => {
    if (client) {
      client.onConnect = () => {
        client.subscribe(`/topic/stream/${streamId}`, (message) => {
          if (remoteVideoRef.current && message.body) {
            const blob = new Blob([new Uint8Array(JSON.parse(message.body))], {
              type: "image/jpeg",
            });
            const url = URL.createObjectURL(blob);
            remoteVideoRef.current.src = url;
          }
        });
      };
      client.activate();
    }
  }, [client, streamId]);

  return (
    <div>
      <h1>Xem Livestream</h1>
      <video ref={remoteVideoRef} autoPlay />
    </div>
  );
};

export default WatchLivestream;
