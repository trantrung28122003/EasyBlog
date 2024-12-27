import React, { useEffect, useRef } from "react";

const LivestreamReceiver: React.FC = () => {
  const videoRef = useRef<HTMLVideoElement | null>(null);
  const socketRef = useRef<WebSocket | null>(null);

  useEffect(() => {
    // Mở kết nối WebSocket
    socketRef.current = new WebSocket("ws://localhost:8080/liveStream");

    socketRef.current.onopen = () => {
      console.log("WebSocket connected");
    };

    socketRef.current.onmessage = (event) => {
      if (event.data instanceof Blob) {
        // Nhận frame video dưới dạng Blob và hiển thị
        const videoUrl = URL.createObjectURL(event.data);
        if (videoRef.current) {
          videoRef.current.src = videoUrl;
        }
      }
    };

    return () => {
      if (socketRef.current) {
        socketRef.current.close();
      }
    };
  }, []);

  return <video ref={videoRef} autoPlay controls />;
};

export default LivestreamReceiver;
