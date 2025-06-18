import React from "react";
import { Outlet, useNavigate } from "react-router-dom";
import { ConfigProvider, Grid, Layout } from "antd";
import AppHeader from "@components/AppHeader";
import { setNavigator } from "@stores/app.store";
import { appTheme } from "./theme";

const { Content, Footer } = Layout;

const DefaultLayout: React.FC = () => {
  const navigate = useNavigate();
  const { sm } = Grid.useBreakpoint();

  React.useEffect(() => {
    setNavigator(navigate);
  }, [navigate]);

  return (
    <ConfigProvider theme={appTheme}>
      <div className="main-container">
        <Layout style={{ backgroundColor: "transparent" }}>
          <Layout style={{ backgroundColor: "transparent" }}>
            <Content className="app-content">
              <AppHeader />
              <div className="outlet-content">
                <Outlet />
              </div>
            </Content>
          </Layout>
          <Footer style={{ fontSize: "8pt", padding: "10px 24px" }}></Footer>
        </Layout>
      </div>
    </ConfigProvider>
  );
};

export default DefaultLayout;
