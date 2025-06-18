import React, { ReactNode } from "react";
import { Button, Col, Divider, Row, Space } from "antd";
import GlobalLoader from "@components/GlobalLoader";
import { setHeaderTitle } from "@stores/app.store";
import "./page.scss";

export interface PageAction {
  key: string;
  action: () => void;
  label?: React.ReactNode;
  icon?: React.ReactNode;
  primary?: boolean;
}

export interface PageProps {
  loading?: boolean;
  actions?: PageAction[];
  title?: string;
  subTitle?: string;
  backPath?: string;
  children: ReactNode | ReactNode[] | undefined;
}

const Page: React.FC<PageProps> = (props: PageProps) => {
  const { loading, title, subTitle, actions, children } = props;

  React.useEffect(() => {
    setHeaderTitle(title ?? "");
    document.title = `BOT[6] - Dex-monitor${title ? ` - ${title}` : ""}`;
  }, [title]);

  return (
    <Row gutter={[16, 16]} className="app-page">
      <Col span={24}>
        <Row style={{ justifyContent: "space-between" }}>
          <h3 className="f16 ts10 ls10">{title}</h3>
          {actions && (
            <Space style={{ marginRight: "40px" }}>
              {actions.map((e) => (
                <Button
                  size="small"
                  type={e.primary ? "primary" : undefined}
                  key={e.key}
                  icon={e.icon}
                  onClick={() => e.action()}
                  className="f10"
                >
                  {e.label}
                </Button>
              ))}
            </Space>
          )}
          <Divider />
        </Row>
        <Row>
          {subTitle && !subTitle.match(/^\s+$/) && (
            <h4 className="f14 bs10 ts10 ls10" style={{ fontWeight: 500 }}>
              {subTitle}
            </h4>
          )}
        </Row>
        <Row>
          <div className="page-item">
            {loading !== undefined && loading ? (
              <GlobalLoader size="150px" message={"Loading"} />
            ) : (
              children
            )}
          </div>
        </Row>
      </Col>
    </Row>
  );
};

export default Page;
