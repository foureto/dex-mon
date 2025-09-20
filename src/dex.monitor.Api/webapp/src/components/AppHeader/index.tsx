import React from "react";
import { useNavigate } from "react-router-dom";
import { Menu } from "antd";
import "./menu.scss";

const items = [
  { key: "main", label: "main" },
  { key: "chains", label: "chains" },
  { key: "dexs", label: "dex" },
  { key: "tokens", label: "tokens" },
];

const AppHeader: React.FC = () => {
  const navigate = useNavigate();
  const [current, setCurrent] = React.useState("main");

  const onClick = (e: { key: string }) => {
    setCurrent(e.key);
    navigate(e.key);
  };

  return (
    <div className="header-menu">
      <div className="menu-nav">
        <Menu
          style={{ width: "100%", background: "transparent" }}
          mode="horizontal"
          onClick={(e) => onClick(e)}
          selectedKeys={[current]}
          items={items}
        />
      </div>
      <div></div>
    </div>
  );
};

export default AppHeader;
