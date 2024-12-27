import React, { useEffect, useRef, useState } from "react";
import { getWebSocketClient } from "../../../hooks/websocket";
import { DoCallAPIWithToken } from "../../../services/HttpService";
import { BASE_URL } from "../../../constants/API";

const Livestream: React.FC = () => {
  const videoRef = useRef<HTMLVideoElement>(null);
  const [streams, setStreams] = useState<any[]>([]); // Danh sách livestreams
  const client = getWebSocketClient();
  const [isStreaming, setIsStreaming] = useState(false);

  // Hàm để lấy danh sách livestreams từ API
  const fetchLiveStreams = async () => {
    try {
      const response = await DoCallAPIWithToken("/api/liveStreams", "get"); // API lấy danh sách livestreams
      setStreams(response.data);
    } catch (error) {
      console.error("Error fetching live streams:", error);
    }
  };

  // Bắt đầu livestream
  const startLivestream = async () => {
    // Tiêu đề livestream

    // Thông báo đến server qua WebSocket rằng livestream đã bắt đầ

    setIsStreaming(true);

    // Lấy video từ camera và bắt đầu stream
    startCameraStream();
  };

  // Hàm để lấy video từ camera và gửi qua WebSocket
  const startCameraStream = async () => {
    const stream = await navigator.mediaDevices.getUserMedia({ video: true });
    if (videoRef.current) {
      videoRef.current.srcObject = stream;
    }

    const canvas = document.createElement("canvas");
    const context = canvas.getContext("2d");

    // Gửi từng khung hình video qua WebSocket
    const sendFrame = () => {
      if (videoRef.current && client?.connected) {
        canvas.width = videoRef.current.videoWidth;
        canvas.height = videoRef.current.videoHeight;
        context?.drawImage(videoRef.current, 0, 0, canvas.width, canvas.height);
        canvas.toBlob((blob) => {
          if (blob) {
            const reader = new FileReader();
            reader.onload = () => {
              if (reader.result) {
                // Gửi khung hình dưới dạng byte array qua WebSocket
                client.publish({
                  destination: "/app/stream", // Địa chỉ WebSocket server để gửi video frame
                  body: reader.result as string,
                });
              }
            };
            reader.readAsArrayBuffer(blob);
          }
        }, "image/jpeg");
      }
    };

    // Gửi khung hình mỗi 100ms
    setInterval(sendFrame, 100);
  };

  useEffect(() => {
    if (client) {
      client.onConnect = () => {
        client.subscribe("/topic/liveStreams", (message) => {
          // Xử lý video khung hình nhận được
          const videoFrame = message.body;
          // Xử lý video frame (hiển thị hoặc lưu trữ)
        });
      };
      client.activate();
    }
  }, [client]);

  return (
    <div>
      <h1>Livestream Page</h1>

      {isStreaming ? (
        <div>
          <h2>Your Stream is live!</h2>
          <video ref={videoRef} autoPlay muted />
        </div>
      ) : (
        <button onClick={startLivestream}>Start Livestream</button>
      )}

      <h3>Livestream List</h3>
      <ul>
        {streams.map((stream) => (
          <li key={stream.streamId}>
            {stream.username} đang livestream
            <video autoPlay style={{ width: "300px", marginTop: "10px" }} />
          </li>
        ))}
      </ul>
    </div>
  );
};

export default Livestream;
