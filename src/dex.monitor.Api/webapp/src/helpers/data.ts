import dayjs from "dayjs";
import { UiEnum } from "@services/commons";

export const flagCode = (iso: string) => {
  const isoNative = iso.toLowerCase().split("-")[0];
  if (isoNative === "en") {
    return "gb";
  }
  if (isoNative === "uk") {
    return "ua";
  }
  return isoNative;
};

export const toNum = (e: string | number): number => {
  if (!e) return 0;
  return Number(e.toString().replace(" ", ""));
};

export const nameFromEnum = (enm: UiEnum, value: number) => {
  const key = Object.getOwnPropertyNames(enm).find(
    (e) => enm[e].value === value
  );
  return key ? enm[key].name : "n-a";
};

export const colorFromEnum = (enm: UiEnum, value: number) => {
  const key = Object.getOwnPropertyNames(enm).find(
    (e) => enm[e].value === value
  );
  return key ? enm[key].options?.color : "default";
};

export const keyFromEnum = (enm: UiEnum, value: number) => {
  return (
    Object.getOwnPropertyNames(enm).find((e) => enm[e].value === value) ?? "n-a"
  );
};

export const formDateTime = (value: string) => {
  if (!value) return "";
  return dayjs(value).format("DD.MM.YY HH:mm");
};

export const shorterText = (value: string, start: number, end: number) => {
  if (!value) return "";
  return `${value.substring(0, start)}...${value.substring(value.length - end - 1, value.length - 1)}`;
};
