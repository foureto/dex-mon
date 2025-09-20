import React from "react";
import Page from "@components/Page";
import { useUnit } from "effector-react";
import ItemsTable from "@components/ItemsTable";
import { $list, fetch, reset } from "./tokens.store";
import { TokenInfo } from "@services/tokens/model";
import Flag from "@components/Flag";

const columns = [
  { title: "Symbol", dataIndex: "code" },
  { title: "Name", dataIndex: "name" },
  {
    title: "Valueble",
    render: (e: TokenInfo) => <Flag value={e.isValuable} />,
    width: 100,
  },
  {
    title: "Dex synced",
    render: (e: TokenInfo) => <Flag value={e.dexSynced} />,
    width: 100,
  },
];

const TokensPage = () => {
  const { data, filter, loading } = useUnit($list);

  React.useEffect(() => {
    if (data === null) fetch();
    return () => reset();
  }, []);

  return (
    <Page title="Tokens" loading={loading}>
      <ItemsTable
        loading={loading}
        columns={columns}
        data={data}
        filter={filter}
        rowKey={(e: TokenInfo) => e.code}
      />
    </Page>
  );
};

export default TokensPage;
