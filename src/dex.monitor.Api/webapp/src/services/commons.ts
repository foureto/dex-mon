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
