export class TokensFilter {
  isValuable?: boolean | null = true;
  cexs?: string[] = [];
  dexs?: string[] = [];
}

export interface DexPair {
  pairAddress: string;
  baseAddress: string;
  quotedAddress: string;
  dexName: string;
  baseSymbol: string;
  quotedSymbol: string;
}

export interface TokenNetwork {
  isValuable: boolean;
  code: string;
  name: string;
  network: string;
  address: string;
  decimals: number;
  pairs: DexPair[];
}

export interface TokenInfo {
  isValuable: boolean;
  dexSynced: boolean;
  code: string;
  name: string;
  cexes: string[];
  networks: string[];
}
