import ChainsService from "@services/chains/ChainsService";
import {
  AddChainRequest,
  ChainStatus,
  UpdateBlockRequest,
  UpdateChainRequest,
} from "@services/chains/models";
import {
  combine,
  createEffect,
  createEvent,
  createStore,
  sample,
} from "effector";

const resetChains = createEvent();
const getChains = createEvent();
const addChain = createEvent<AddChainRequest>();
const updateChain = createEvent<UpdateChainRequest>();
const setBlock = createEvent<UpdateBlockRequest | null>();
const updateBlock = createEvent<UpdateBlockRequest>();

const setCurrent = createEvent<ChainStatus | null>();
const showDialog = createEvent<boolean>();

const getChainsFx = createEffect(() => ChainsService.getChains());
const addChainFx = createEffect((req: AddChainRequest) =>
  ChainsService.addChain(req)
);
const updateChainFx = createEffect((req: UpdateChainRequest) =>
  ChainsService.updateChain(req)
);

const updateBlockFx = createEffect((req: UpdateBlockRequest) =>
  ChainsService.updateBlock(req)
);

const $dialogOpen = createStore<boolean>(false).on(showDialog, (_, e) => e);

sample({ clock: getChains, target: getChainsFx });
sample({ clock: addChain, target: addChainFx });
sample({ clock: updateChain, target: updateChainFx });
sample({ clock: updateBlock, target: updateBlockFx });

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
  clock: [updateChainFx.done, addChainFx.done, updateBlockFx.done],
  fn: () => false,
  target: [resetChains, showDialog],
});

const $loading = createStore<boolean>(false)
  .on(getChainsFx.pending, (_, e) => e)
  .reset(resetChains);

const $chainsList = createStore<ChainStatus[] | null>(null)
  .on(getChainsFx.doneData, (_, e) => e.data ?? [])
  .reset(resetChains);

const $current = createStore<ChainStatus | null>(null)
  .on(setCurrent, (_, e) => e)
  .reset(resetChains);

const $currentBlock = createStore<UpdateBlockRequest | null>(null)
  .on(setBlock, (_, e) => e)
  .reset(resetChains);

const $error = createStore<string | null>(null)
  .on(addChainFx.failData, (_, e) => e.message)
  .on(updateChainFx.failData, (_, e) => e.message)
  .reset([updateChainFx.done, addChainFx.done, resetChains]);

const $chains = combine({
  list: $chainsList,
  loading: $loading,
  dialogOpen: $dialogOpen,
  current: $current,
  currentBlock: $currentBlock,
  error: $error,
});

export {
  $chains,
  getChains,
  addChain,
  updateChain,
  resetChains,
  setCurrent,
  showDialog,
  setBlock,
  updateBlock,
};
