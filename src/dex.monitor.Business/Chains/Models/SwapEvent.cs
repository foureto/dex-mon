using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace dex.monitor.Business.Chains.Models;

[Event("Swap")]
public class SwapEvent : IEventDTO
{
    [Parameter("address", "sender", 1, true)]
    public string Sender { get; set; } = string.Empty;

    [Parameter("uint256", "amount0In", 2, false)]
    public BigInteger Amount0In { get; set; }

    [Parameter("uint256", "amount1In", 3, false)]
    public BigInteger Amount1In { get; set; }

    [Parameter("uint256", "amount0Out", 4, false)]
    public BigInteger Amount0Out { get; set; }

    [Parameter("uint256", "amount1Out", 5, false)]
    public BigInteger Amount1Out { get; set; }

    [Parameter("address", "to", 6, true)] public string To { get; set; } = string.Empty;
}