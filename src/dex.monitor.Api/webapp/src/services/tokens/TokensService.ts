import { IListResult } from "@services/commons";
import { ServiceBase } from "@services/ServiceBase";
import { TokenInfo, TokensFilter } from "./model";

class TokensService extends ServiceBase {
  protected static PREFIX = "tokens";

  public static getTokens(req: TokensFilter): Promise<IListResult<TokenInfo>> {
    return this.get("", req);
  }
}

export default TokensService;
