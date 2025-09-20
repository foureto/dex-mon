import React from "react";
import GlobalLoader from "@components/GlobalLoader";
import { Table } from "antd";

export interface ItemsTableProps<TF, TRes> {
  loading?: boolean;
  data: TRes[] | null;
  columns: any[];
  rowKey?: any;
  filter: TF | null;
}

const ItemsTable = <TF extends object, TRes extends object>(
  props: ItemsTableProps<TF, TRes>
) => {
  const { loading, data, columns, rowKey, filter } = props;

  if (filter) {
    console.log(Object.getOwnPropertyNames(filter));
  }

  return (
    <>
      <Table
        loading={loading && { indicator: <GlobalLoader size="40px" /> }}
        dataSource={data ?? []}
        columns={columns}
        rowKey={rowKey}
        pagination={{
          size: "small",
          defaultPageSize: 25,
          pageSizeOptions: ["25", "50", "100", "1000"],
          showSizeChanger: true,
          className: "p10",
        }}
      />
    </>
  );
};

export default ItemsTable;
