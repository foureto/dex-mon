using System.Numerics;
using dex.monitor.Business.Domain;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace dex.monitor.Business.Chains.Filters;

[Function("swapExactTokensForTokensSupportingFeeOnTransferTokens", "uint[]")]
public class SwapExactTokensForTokensFunction : FunctionMessage
{
    [Parameter("uint256", "amountIn", 1)] public BigInteger AmountIn { get; set; }

    [Parameter("uint256", "amountOutMin", 2)]
    public BigInteger AmountOutMin { get; set; }

    [Parameter("address[]", "path", 3)] public List<string> Path { get; set; }

    [Parameter("address", "to", 4)] public string To { get; set; }

    [Parameter("uint256", "deadline", 5)] public BigInteger Deadline { get; set; }
}

internal class PancakeFilterParser : ITransactionFilterParser
{
    public string Network => ChainConstants.Bsc;

    public bool Acceptable(Transaction transaction)
        => transaction.IsTransactionForFunctionMessage<SwapExactTokensForTokensFunction>();

    public async Task<List<SwapOperation>> Process(IChainProvider provider, Transaction tx, CancellationToken ct)
    {
        var message = tx.DecodeTransactionToFunctionMessage<SwapExactTokensForTokensFunction>();
        var from = await provider.GetTokenInfo(message.Path[0], ct);
        var to = await provider.GetTokenInfo(message.Path[1], ct);
        
        var result = new SwapOperation
        {
            Network = provider.Network,
            RouterAddress = tx.To,
            Pair = new TokenSwapPair
            {
                PairAddress = "",
                Symbol = "",
                In = new TokenSwapValue
                {
                    Address = from.Address,
                    Code = from.Code,
                    Value = 0m,
                },
                Out = new TokenSwapValue
                {
                    Address = to.Address,
                    Code = to.Code,
                    Value = 0m,
                },
            }
        };


        return Task.FromResult<List<SwapOperation>>([result]);
    }
}