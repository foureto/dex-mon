import React from "react";

import "./ItemRow.scss";

export interface ItemRowProps {
  icon?: string;
  ticker?: string;
  text?: string;
  amount?: string;
  currencyAmount?: string;
  onClick?: () => void;
}

const ItemRow: React.FC<ItemRowProps> = ({
  icon,
  ticker,
  text,
  amount,
  currencyAmount,
  onClick,
}) => {
  return (
    <div className={`item-row`} onClick={() => onClick?.()}>
      <div className="item-icon">
        <img src={icon} alt="icon" />
      </div>
      <div className="item-description">
        <span className="item-ticker">{ticker}</span>
        <span className="item-name f10">{text}</span>
      </div>

      <div className="item-amount">
        <div>
          {amount} {ticker}
        </div>
        <div className="item-amount-currency">$ {currencyAmount}</div>
      </div>
    </div>
  );
};

export default ItemRow;
