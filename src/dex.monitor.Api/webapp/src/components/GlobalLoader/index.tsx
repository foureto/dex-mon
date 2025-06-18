import React from "react";
import "./GlobalLoader.scss";

export interface GlobalErrorProps {
  global?: boolean;
  message?: string;
  size?: string;
  color?: string;
}

const GlobalLoader: React.FC<GlobalErrorProps> = ({
  message,
  size,
  global,
  color,
}) => {
  const style = size ? { width: size, height: size } : {};
  return (
    <div className={`e-container ${global ? "abs-loader" : ""}`}>
      <div className="square" style={style}>
        <span style={{ borderColor: color ?? "black" }}></span>
        <span style={{ borderColor: color ?? "black" }}></span>
        <span style={{ borderColor: color ?? "black" }}></span>
        <div className="e-message">{message}</div>
      </div>
    </div>
  );
};

export default GlobalLoader;
