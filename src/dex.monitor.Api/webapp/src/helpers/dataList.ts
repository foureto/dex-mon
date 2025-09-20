import { combine, createStore, createEvent, Effect, sample } from "effector";
import { IListResult } from "@services/commons";
import { debounce } from "patronum";

export const getDataList = <TF, TRes>(
  handler: Effect<TF, IListResult<TRes>, Error>,
  defaultFilter?: TF,
  keySelector?: (e: TRes) => any,
  transform?: (e: TRes) => TRes
) => {
  // events
  const reset = createEvent();
  const fetch = createEvent();
  const setFilter = createEvent<TF>();
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

  const $filter = createStore<TF | null>(
    defaultFilter ? defaultFilter : null
  ).on(setFilter, (s, e) => ({ ...s, ...e }));

  const $loading = handler.pending;

  sample({
    clock: debounceFetch,
    source: {
      filter: $filter,
    },
    fn: ({ filter }) => filter as TF,
    target: handler,
  });

  sample({ clock: setFilter, target: fetch });

  const $list = combine({
    data: $data,
    filter: $filter,
    loading: $loading,
  });

  return { $list, reset, fetch, setFilter };
};
