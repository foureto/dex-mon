import DexService from "@services/dexs/DexService";
import {
  AddDexRequest,
  DexSetting,
  UpdateDexRequest,
} from "@services/dexs/models";
import {
  combine,
  createEffect,
  createEvent,
  createStore,
  sample,
} from "effector";

const resetDexs = createEvent();
const getDexs = createEvent();
const addDex = createEvent<AddDexRequest>();
const updateDex = createEvent<UpdateDexRequest>();

const setCurrent = createEvent<DexSetting | null>();
const showDialog = createEvent<boolean>();

const getDexsFx = createEffect(() => DexService.getDexs());
const addDexsFx = createEffect((req: AddDexRequest) => DexService.addDex(req));
const updateDexsFx = createEffect((req: UpdateDexRequest) =>
  DexService.updateDex(req)
);
const $dialogOpen = createStore<boolean>(false).on(showDialog, (_, e) => e);

sample({ clock: getDexs, target: getDexsFx });
sample({ clock: addDex, target: addDexsFx });
sample({ clock: updateDex, target: updateDexsFx });

sample({
  clock: setCurrent,
  filter: (e) => !!e,
  fn: () => true,
  target: showDialog,
});

sample({
  clock: showDialog,
  filter: (e) => !e,
  fn: () => null,
  target: setCurrent,
});

sample({
  clock: [updateDexsFx.done, addDexsFx.done],
  fn: () => false,
  target: [resetDexs, showDialog],
});

const $loading = createStore<boolean>(false)
  .on(getDexsFx.pending, (_, e) => e)
  .reset(resetDexs);

const $dexsList = createStore<DexSetting[] | null>(null)
  .on(getDexsFx.doneData, (_, e) => e.data ?? [])
  .reset(resetDexs);

const $current = createStore<DexSetting | null>(null)
  .on(setCurrent, (_, e) => e)
  .reset(resetDexs);

const $error = createStore<string | null>(null)
  .on(addDexsFx.failData, (_, e) => e.message)
  .on(updateDexsFx.failData, (_, e) => e.message)
  .reset([updateDexsFx.done, addDexsFx.done, resetDexs]);

const $dex = combine({
  list: $dexsList,
  loading: $loading,
  dialogOpen: $dialogOpen,
  current: $current,
  error: $error,
});

export { $dex, getDexs, addDex, updateDex, resetDexs, setCurrent, showDialog };
