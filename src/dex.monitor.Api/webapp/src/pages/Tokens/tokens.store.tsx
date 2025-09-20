import { createEffect } from "effector";
import { getDataList } from "@helpers/dataList";
import { TokenInfo, TokensFilter } from "@services/tokens/model";
import TokensService from "@services/tokens/TokensService";

const getTokensFx = createEffect((req: TokensFilter) =>
  TokensService.getTokens(req)
);

export const { $list, reset, fetch, setFilter } = getDataList<
  TokensFilter,
  TokenInfo
>(getTokensFx, new TokensFilter(), (e) => e.code);
