import React from "react";
import { useUnit } from "effector-react";
import { Divider, Form, Input, Modal, Select, Switch } from "antd";
import { EditOutlined } from "@ant-design/icons";
import { ChainStatus } from "@services/chains/models";
import Page from "@components/Page";
import ItemsList from "@components/ItemsList";
import {
  $chains,
  addChain,
  getChains,
  resetChains,
  setBlock,
  setCurrent,
  showDialog,
  updateBlock,
  updateChain,
} from "./chains.store";
import "./styles.scss";

const ChainEditDialog: React.FC = () => {
  const { dialogOpen, current, error } = useUnit($chains);
  const [form] = Form.useForm();

  const save = () => {
    const values = form.getFieldsValue();
    if (current === null) {
      addChain(values);
      return;
    }

    updateChain({ ...values, ...{ id: current.id } });
  };

  const netOptions = React.useMemo(() => {
    return ["bsc", "celo"].map((e) => ({ value: e, label: e.toUpperCase() }));
  }, []);

  React.useEffect(() => {
    if (!!current) form.setFieldsValue(current);
  }, [current]);

  return (
    <Modal
      destroyOnHidden
      title={"Add Chain"}
      open={dialogOpen}
      onOk={() => form.submit()}
      onCancel={() => showDialog(false)}
      afterClose={() => form.resetFields()}
    >
      <Form form={form} onFinish={save} labelCol={{ span: 6 }}>
        {!!current && (
          <Form.Item name={"isActive"} label={"Is active"}>
            <Switch />
          </Form.Item>
        )}
        <Form.Item
          name={"name"}
          label={"Name"}
          rules={[{ required: true, message: "Name is required" }]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          name={"network"}
          label={"Network"}
          rules={[{ required: true, message: "Network is required" }]}
          initialValue={"bsc"}
        >
          <Select options={netOptions} />
        </Form.Item>
        <Form.Item
          name={"apiUrl"}
          label={"RPC URL"}
          rules={[{ required: true, message: "RPC URL is required" }]}
        >
          <Input />
        </Form.Item>
        <Form.Item name={"wsUrl"} label={"Websocket URL"}>
          <Input />
        </Form.Item>
      </Form>
      {error && <div className="error-hint">{error}</div>}
    </Modal>
  );
};

const ChainBlockEditDialog: React.FC = () => {
  const { currentBlock } = useUnit($chains);
  const [form] = Form.useForm();

  const save = (values: any) => {
    if (!currentBlock) return;
    updateBlock({ chainId: currentBlock.chainId, height: values.height });
  };

  React.useEffect(() => {
    if (!!currentBlock) form.setFieldsValue(currentBlock);
  }, [currentBlock]);

  return (
    <Modal
      destroyOnHidden
      title={"Update block height"}
      open={!!currentBlock}
      onOk={() => form.submit()}
      onCancel={() => setBlock(null)}
      afterClose={() => form.resetFields()}
    >
      <Form form={form} onFinish={save}>
        <Form.Item name={"height"} label="Block height">
          <Input type="number" />
        </Form.Item>
      </Form>
    </Modal>
  );
};

const ChainsPage: React.FC = () => {
  const { list, loading } = useUnit($chains);

  React.useEffect(() => {
    if (list === null) getChains();
  }, [list]);

  React.useEffect(() => {
    return () => resetChains();
  }, []);

  return (
    <Page
      title="Dexs"
      actions={[
        {
          key: "add",
          label: "add chain",
          action: () => showDialog(true),
        },
      ]}
    >
      <ItemsList
        useBlock
        rootClassName="chains-list"
        data={list ?? []}
        loading={loading}
        renderRow={(e: ChainStatus) => (
          <div className="chian-item">
            <div>
              <label></label>
              <b>{e.name}</b>
              <span>
                {" "}
                <EditOutlined color="volcano" onClick={() => setCurrent(e)} />
              </span>
            </div>
            <Divider />
            <div>
              <label>Is active:</label>
              <b>{e.isActive ? "Yes" : "No"}</b>
            </div>
            <div>
              <label>Network:</label>
              <b>{e.network}</b>
            </div>
            <div>
              <label>RPC URL:</label>
              <b>{e.apiUrl}</b>
            </div>
            <div>
              <label>WebSocket URL:</label>
              <b>{e.wsUrl}</b>
            </div>
            <div>
              <label>Block height:</label>
              <b>{e.block?.height}</b>
              <span>
                {" "}
                <EditOutlined
                  color="volcano"
                  onClick={() =>
                    setBlock({ chainId: e.id, height: e.block?.height ?? 0 })
                  }
                />
              </span>
            </div>
          </div>
        )}
      />
      <ChainEditDialog />
      <ChainBlockEditDialog />
    </Page>
  );
};

export default ChainsPage;
