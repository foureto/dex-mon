import { ServiceBase } from "@services/ServiceBase";
import { IListResult, IResult } from "@services/commons";
import {
  AddChainRequest,
  ChainStatus,
  UpdateBlockRequest,
  UpdateChainRequest,
} from "./models";

class ChainsService extends ServiceBase {
  protected static BASE_URL = "chains/";

  public static getChains(): Promise<IListResult<ChainStatus>> {
    return this.get("");
  }

  public static addChain(req: AddChainRequest): Promise<IResult> {
    return this.post("", req);
  }

  public static updateChain(req: UpdateChainRequest): Promise<IResult> {
    return this.put("", req);
  }

  public static updateBlock(req: UpdateBlockRequest): Promise<IResult> {
    return this.put("block", req);
  }
}

export default ChainsService;
