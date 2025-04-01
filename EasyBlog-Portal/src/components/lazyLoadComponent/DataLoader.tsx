import React from "react";
import "./DataLoader.css";

interface DataLoaderProps {
  isLoading: boolean;
  isOpaque?: boolean;
  message?: string;
}

const DataLoader: React.FC<DataLoaderProps> = ({
  isLoading,
  isOpaque = false,
  message = "Đang tải dữ liệu...",
}) => {
  if (!isLoading) return null;

  const overlayStyle = {
    "--overlay-opacity": isOpaque ? 0.4 : 1,
  } as React.CSSProperties;

  return (
    <div className="data-loader-overlay" style={overlayStyle}>
      <div className="spinner"></div>
      <p className="loading-message">{message}</p>
    </div>
  );
};

export default DataLoader;
