import GlobalLoader from "@components/GlobalLoader";
import { Table } from "antd";

export interface ItemsPagedTableProps<TRes> {
  loading?: boolean;
  data: TRes[] | null;
  columns: any[];
  total: number;
  page: number;
  pageSize: number;
  paginationChanged?: (req: { page: number; pageSize: number }) => void;
  rowKey?: any;
}

const ItemsPagedTable = <TRes extends object>(props: ItemsPagedTableProps<TRes>) => {
  const {
    loading,
    data,
    columns,
    total = 0,
    page = 0,
    pageSize = 0,
    paginationChanged,
    rowKey,
  } = props;

  return (
    <Table
      loading={loading && { indicator: <GlobalLoader size="40px" /> }}
      dataSource={data ?? []}
      columns={columns}
      rowKey={rowKey}
      pagination={
        paginationChanged
          ? {
              size: "small",
              total: total,
              current: page,
              pageSize: pageSize,
              defaultPageSize: 20,
              pageSizeOptions: ["20", "50", "100", "1000"],
              showSizeChanger: true,
              showTotal: (total) => `Total: ${total}`,
              className: "p10",
            }
          : false
      }
      onChange={(pagination) => {
        paginationChanged?.({
          page: pagination.current ?? 1,
          pageSize: pagination.pageSize ?? 50,
        });
      }}
    />
  );
};

export default ItemsPagedTable;
