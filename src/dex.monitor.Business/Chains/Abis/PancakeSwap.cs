namespace dex.monitor.Business.Chains.Abis;

public class PancakeSwap
{
    public const string FactoryAbi = """
                                     [
                                           {
                                             "constant": true,
                                             "inputs": [{ "name": "index", "type": "uint256" }],
                                             "name": "allPairs",
                                             "outputs": [{ "name": "", "type": "address" }],
                                             "payable": false,
                                             "stateMutability": "view",
                                             "type": "function"
                                           },
                                           {
                                             "constant": true,
                                             "inputs": [],
                                             "name": "allPairsLength",
                                             "outputs": [{ "name": "", "type": "uint256" }],
                                             "payable": false,
                                             "stateMutability": "view",
                                             "type": "function"
                                           },
                                           {
                                             "constant": true,
                                             "inputs": [
                                               { "name": "tokenA", "type": "address" },
                                               { "name": "tokenB", "type": "address" }
                                             ],
                                             "name": "getPair",
                                             "outputs": [{ "name": "", "type": "address" }],
                                             "payable": false,
                                             "stateMutability": "view",
                                             "type": "function"
                                           }
                                         ]
                                     """;

    public const string PairAbi = """
                                  [
                                    {"constant":true,"inputs":[],"name":"getReserves","outputs":[{"internalType":"uint112","name":"_reserve0","type":"uint112"},{"internalType":"uint112","name":"_reserve1","type":"uint112"},{"internalType":"uint32","name":"_blockTimestampLast","type":"uint32"}],"payable":false,"stateMutability":"view","type":"function"},
                                    {"constant":true,"inputs":[],"name":"token0","outputs":[{"internalType":"address","name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},
                                    {"constant":true,"inputs":[],"name":"token1","outputs":[{"internalType":"address","name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},
                                    {
                                      "anonymous": false,
                                      "inputs": [
                                        { "indexed": true, "name": "sender", "type": "address" },
                                        { "indexed": false, "name": "amount0In", "type": "uint256" },
                                        { "indexed": false, "name": "amount1In", "type": "uint256" },
                                        { "indexed": false, "name": "amount0Out", "type": "uint256" },
                                        { "indexed": false, "name": "amount1Out", "type": "uint256" }
                                      ],
                                      "name": "Swap",
                                      "type": "event"
                                    }
                                  ]
                                  """;
}