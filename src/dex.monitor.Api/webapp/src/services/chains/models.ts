export interface ChainStatus {
  id: string;
  isActive: boolean;
  name: string;
  network: string;
  apiUrl: string;
  wsUrl?: string;
  created: string;
  updated: string;
  block: {
    height: number;
    updated: string;
  };
}

export interface AddChainRequest {
  name: string;
  network: string;
  apiUrl: string;
  wsUrl?: string;
}

export interface UpdateChainRequest extends AddChainRequest {
  id: string;
  isActive: boolean;
}

export interface UpdateBlockRequest {
  chainId: string;
  height: number;
}
