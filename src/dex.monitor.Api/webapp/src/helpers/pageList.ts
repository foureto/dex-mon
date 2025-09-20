import { combine, createStore, createEvent, Effect, sample } from "effector";
import { HandlerProps, IPagedResult } from "@services/commons";
import { debounce } from "patronum";

const defaultPageSize = 50;

export const getPagedList = <TF, TRes>(
  handler: Effect<HandlerProps<TF>, IPagedResult<TRes>, Error>,
  defaultFilter?: TF,
  keySelector?: (e: TRes) => any,
  transform?: (e: TRes) => TRes
) => {
  // events
  const reset = createEvent();
  const fetch = createEvent();
  const nextPage = createEvent();
  const setPage = createEvent<number>();
  const setFilter = createEvent<TF>();
  const setMore = createEvent<boolean>();
  const debounceFetch = debounce(fetch, 300);

  // stores
  const $data = createStore<TRes[] | null>(null)
    .on(handler.doneData, (s, e) => {
      const res = s ?? [];
      return (e.data ?? [])
        .map((e) => transform?.(e) ?? e)
        .reduce((result: TRes[], cur: TRes) => {
          if (
            !result.find((e) =>
              keySelector ? keySelector(e) === keySelector(cur) : e === cur
            )
          )
            res.push(cur);
          return res;
        }, res);
    })
    .reset(reset);

  const $hasMore = createStore<boolean>(true)
    .on(setMore, (_, e) => e)
    .reset(setFilter);
  const $page = createStore<number>(1)
    .on(handler.doneData, (_, e) => e.page)
    .on(setPage, (_, e) => e)
    .reset(setFilter);
  const $count = createStore<number>(defaultPageSize)
    .on(handler.doneData, (_, e) => e.count)
    .reset(setFilter);
  const $total = createStore<number>(0)
    .on(handler.doneData, (_, e) => e.total)
    .reset(setFilter);
  const $filter = createStore<TF | null>(
    defaultFilter ? defaultFilter : null
  ).on(setFilter, (_, e) => e);

  const $loading = handler.pending;

  sample({
    clock: debounceFetch,
    source: {
      pageIndex: $page,
      pageSize: $count,
      filter: $filter,
    },
    // filter: ({ isTableRendered }) => !isTableRendered,
    fn: ({ pageIndex, pageSize, filter }) => ({
      pageIndex,
      pageSize,
      filter: filter as TF,
    }),
    target: handler,
  });

  sample({
    clock: handler.doneData,
    source: { data: $data, total: $total },
    fn: ({ data, total }) => (data?.length ?? 0) < total,
    target: setMore,
  });

  sample({
    clock: nextPage,
    source: { page: $page, hasMore: $hasMore },
    filter: ({ hasMore }) => hasMore,
    fn: ({ page }) => page + 1,
    target: setPage,
  });

  sample({ clock: setFilter, target: fetch });
  sample({ clock: setPage, target: fetch });

  const $list = combine({
    data: $data,
    filter: $filter,
    loading: $loading,
  });

  return { $list, $hasMore, reset, fetch, setFilter, nextPage };
};
