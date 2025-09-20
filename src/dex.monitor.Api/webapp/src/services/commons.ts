export type HandlerProps<T> = {
  pageIndex: number;
  pageSize: number;
  filter?: T;
  listSort?: SortDescriptor[];
};

export interface Sort {
  field: string;
  order: number;
}

export interface Filter {
  field: string;
  value?: string;
}

export interface IResult {
  statusCode: number;
  message: string;
  success: boolean;
}

export interface IDataResult<T> extends IResult {
  data?: T;
}

export interface IListResult<T> extends IResult {
  data?: T[];
}

export interface IPagedResult<T> extends IResult {
  data?: T[];
  total: number;
  count: number;
  page: number;
}

export interface SortDescriptor {
  field: string;
  order: number;
}

export type UiEnumOptions = { title?: string; color?: string };
export type UiEnum = Record<
  string,
  { name: string; value: number | string; options?: UiEnumOptions }
>;

export const transformFilter = (filter: Filter[]): any => {
  const result = {};
  if (filter && filter.length > 0) {
    for (let i = 0; i < filter.length; i++) {
      // eslint-disable-next-line @typescript-eslint/ban-ts-comment
      // @ts-ignore
      result[filter[i].field] = filter[i].value;
    }
  }
  return result;
};

export type Nullable<T> = null | undefined | T;
export type AnyObject<T = any> = Record<PropertyKey, T>;

export class JSONHelper {
  public static tryParse = <T = unknown>(input: string): T | null => {
    if (input === undefined || input === null) return null;
    if (typeof input !== "string") return null;

    try {
      return JSON.parse(input) as T;
    } catch (err) {
      console.error("Error tryParse: ", err);
      return null;
    }
  };

  public static tryStringify = (value: any, key: string): string | null => {
    if (value === undefined || value === null) return null;
    if (typeof value === "string") return value;

    try {
      return JSON.stringify(value);
    } catch (err) {
      console.error(`Error serializing value for key [${key}]:`, err);
      return null;
    }
  };
}

export function isFormData(value: unknown): value is FormData {
  try {
    // Native instanceof check (works in same realm/browser)
    if (typeof FormData !== "undefined" && value instanceof FormData) {
      return true;
    }

    return (
      value !== null &&
      typeof value === "object" &&
      typeof (value as FormData).append === "function" &&
      Object.prototype.toString.call(value) === "[object FormData]"
    );
  } catch (error) {
    console.error(error);
    return false;
  }
}
