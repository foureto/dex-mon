import React from "react";
import { useUnit } from "effector-react";
import { Divider, Form, Input, Modal, Select, Switch } from "antd";
import { EditOutlined } from "@ant-design/icons";
import { DexSetting } from "@services/dexs/models";
import Page from "@components/Page";
import ItemsList from "@components/ItemsList";
import {
  $dex,
  addDex,
  getDexs,
  resetDexs,
  setCurrent,
  showDialog,
  updateDex,
} from "./dex.store";
import "./styles.scss";

const DesxEditDialog: React.FC = () => {
  const { dialogOpen, current, error } = useUnit($dex);
  const [form] = Form.useForm();

  const save = () => {
    const values = form.getFieldsValue();
    if (current === null) {
      addDex(values);
      return;
    }

    updateDex({ ...values, ...{ id: current.id } });
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
      title={"Add DEX"}
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
          rules={[{ required: true, message: "Name is required" }]}
          initialValue={"bsc"}
        >
          <Select options={netOptions} />
        </Form.Item>
        <Form.Item
          name={"routerAddress"}
          label={"Router"}
          rules={[{ required: true, message: "Router address is required" }]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          name={"factoryAddress"}
          label={"Factory"}
          rules={[{ required: true, message: "Factory address is required" }]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          name={"fee"}
          label={"Fee"}
          rules={[{ required: true, message: "Fee is required" }]}
        >
          <Input type="number" />
        </Form.Item>
      </Form>
      {error && <div className="error-hint">{error}</div>}
    </Modal>
  );
};

const DexsPage: React.FC = () => {
  const { list, loading } = useUnit($dex);

  React.useEffect(() => {
    if (list === null) getDexs();
  }, [list]);

  React.useEffect(() => {
    return () => resetDexs();
  }, []);

  return (
    <Page
      title="Dexs"
      actions={[
        {
          key: "add",
          label: "add dex",
          action: () => showDialog(true),
        },
      ]}
    >
      <ItemsList
        useBlock
        rootClassName="dex-list"
        data={list ?? []}
        loading={loading}
        renderRow={(e: DexSetting) => (
          <div className="dex-item">
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
              <label>Factory:</label>
              <b>{e.factoryAddress}</b>
            </div>
            <div>
              <label>Router:</label>
              <b>{e.routerAddress}</b>
            </div>
            <div>
              <label>Fee:</label>
              <b>{e.fee}</b>
            </div>
          </div>
        )}
      />
      <DesxEditDialog />
    </Page>
  );
};

export default DexsPage;
