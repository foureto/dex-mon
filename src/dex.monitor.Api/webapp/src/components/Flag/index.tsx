import { Tag } from "antd";
import React from "react";

const Flag: React.FC<{ value: boolean }> = ({ value }) => {
  return <Tag color={value ? "green" : "volcano"}>{value ? "Yes" : "No"}</Tag>;
};

export default Flag;
