import { ServiceBase } from "@services/ServiceBase";
import { IListResult, IResult } from "@services/commons";
import { AddDexRequest, DexSetting, UpdateDexRequest } from "./models";

class DexService extends ServiceBase {
  protected static BASE_URL = "dexs";

  public static getDexs(): Promise<IListResult<DexSetting>> {
    return this.get("");
  }

  public static addDex(req: AddDexRequest): Promise<IResult> {
    return this.post("", req);
  }

  public static updateDex(req: UpdateDexRequest): Promise<IResult> {
    return this.put("", req);
  }
}

export default DexService;
