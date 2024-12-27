import React from "react";
import "./DataLoader.css";
const LazyLoadComponent = (lazyComponent: Promise<any>) => {
  const LazyComponent = React.lazy(() => lazyComponent);
  return (
    <React.Suspense
      fallback={
        <div className="data-loader-overlay">
          <div className="spinner"></div>
        </div>
      }
    >
      <LazyComponent />
    </React.Suspense>
  );
};

export default LazyLoadComponent;
