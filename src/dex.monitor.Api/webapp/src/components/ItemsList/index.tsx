import React from "react";
import GlobalLoader from "@components/GlobalLoader";

export interface ItemsListProps<TRes> {
  loading?: boolean;
  data?: TRes[];
  renderRow: (e: TRes) => React.ReactNode;
  rootClassName?: string;
  useBlock?: boolean;
}

const ItemsList = <TRes extends object>(props: ItemsListProps<TRes>) => {
  const { data, loading, renderRow, rootClassName, useBlock } = props;

  if (loading)
    return (
      <div>
        <GlobalLoader size="40px" />
      </div>
    );

  return (
    <div className={rootClassName}>
      {useBlock &&
        (data ?? []).map((e, i) => <div key={`item_${i}`}>{renderRow(e)}</div>)}
      {!useBlock && (
        <ul>
          {(data ?? []).map((e, i) => (
            <li key={`item_${i}`}>{renderRow(e)}</li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default ItemsList;
