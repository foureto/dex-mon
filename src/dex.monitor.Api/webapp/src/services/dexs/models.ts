export interface DexSetting {
  id: string;
  isActive: boolean;
  name: string;
  network: string;
  routerAddress: string;
  factoryAddress: string;
  fee: number;
  created: string;
  updated: string;
}

export interface AddDexRequest {
  name: string;
  network: string;
  routerAddress: string;
  factoryAddress: string;
  fee: number;
}

export interface UpdateDexRequest extends AddDexRequest {
  id: string;
  isActive: boolean;
}
