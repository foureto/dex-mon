using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace dex.monitor.Business.Chains.Abis.Models;

[FunctionOutput]
public class Reserves
{
    [Parameter("uint112", "_reserve0", 1)] public BigInteger ReserveOne { get; set; }
    [Parameter("uint112", "_reserve1", 2)] public BigInteger ReserveTwo { get; set; }
    [Parameter("uint32", "_blockTimestampLast", 3)] public long TimeStamp { get; set; }
}