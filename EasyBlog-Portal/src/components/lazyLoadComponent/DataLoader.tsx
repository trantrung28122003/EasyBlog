import React from "react";
import "./DataLoader.css";

interface DataLoaderProps {
  isLoading: boolean;
}

const DataLoader: React.FC<DataLoaderProps> = ({ isLoading }) => {
  if (!isLoading) return null;
  return (
    <div className="data-loader-overlay">
      <div className="spinner"></div>
    </div>
  );
};

export default DataLoader;
