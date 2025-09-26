using System.Numerics;
using dex.monitor.Business.Chains.Models;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;

namespace dex.monitor.Business.Chains.DexProviders;

internal class UniswapV2Provider(Web3 web3, ILogger logger) : IDexChainProvider
{
    private const string RouterAbi
        = """
          [
              {"inputs":[{"internalType":"uint256","name":"amountIn","type":"uint256"},{"internalType":"address[]","name":"path","type":"address[]"}],"name":"getAmountsOut","outputs":[{"internalType":"uint256[]","name":"amounts","type":"uint256[]"}],"stateMutability":"view","type":"function"},
              {"inputs":[{"internalType":"uint256","name":"amountOut","type":"uint256"},{"internalType":"address[]","name":"path","type":"address[]"}],"name":"getAmountsIn","outputs":[{"internalType":"uint256[]","name":"amounts","type":"uint256[]"}],"stateMutability":"view","type":"function"},
              {"constant":true,"inputs":[],"name":"factory","outputs":[{"internalType":"address","name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"}
          ]
          """;

    private const string FactoryAbi
        = """
          [
              {"inputs":[{"internalType":"address","name":"tokenA","type":"address"},{"internalType":"address","name":"tokenB","type":"address"}],"name":"getPair","outputs":[{"internalType":"address","name":"pair","type":"address"}],"stateMutability":"view","type":"function"}
          ]
          """;

    private const string PairAbi
        = """
          [
              {"inputs":[],"name":"getReserves","outputs":[{"internalType":"uint112","name":"reserve0","type":"uint112"},{"internalType":"uint112","name":"reserve1","type":"uint112"},{"internalType":"uint32","name":"blockTimestampLast","type":"uint32"}],"stateMutability":"view","type":"function"},
              {"inputs":[],"name":"token0","outputs":[{"internalType":"address","name":"","type":"address"}],"stateMutability":"view","type":"function"},
              {"inputs":[],"name":"token1","outputs":[{"internalType":"address","name":"","type":"address"}],"stateMutability":"view","type":"function"}
          ]
          """;

    public async Task<DexPairRate> GetRate(DexPairRequest request, CancellationToken ct = default)
    {
        var price = await GetTokenPriceAsync(
            request.RouterAddress, request.BaseAddress, request.QuotedAddress, ct);
        if (price == null) return null;

        var liquidity = await GetPairLiquidityAsync(
            request.RouterAddress, request.BaseAddress, request.QuotedAddress, ct);

        return new DexPairRate
        {
            BidPrice = price.Value * (1 - 0.003m - 0.002m), // Additional slippage for BSC
            AskPrice = price.Value * (1 + 0.003m + 0.002m),
            Stamp = DateTime.UtcNow,
            Network = "",
            Liquidity = liquidity
        };
    }

    private async Task<decimal?> GetTokenPriceAsync(
        string router, string tokenIn, string tokenOut, CancellationToken ct)
    {
        try
        {
            var routerContract = web3.Eth.GetContract(RouterAbi, router);
            var getAmountsOutFunction = routerContract.GetFunction("getAmountsOut");

            // Use 1 token as base amount (adjust for decimals)
            var amountIn = BigInteger.Parse("1000000000000000000"); // 1 token with 18 decimals
            var path = new[] { tokenIn, tokenOut };

            var amounts = await getAmountsOutFunction.CallAsync<List<BigInteger>>(amountIn, path);

            if (amounts == null || amounts.Count < 2)
            {
                return null;
            }

            // Convert back to decimal (assuming 18 decimals for simplicity)
            var price = (decimal)amounts[1] / (decimal)Math.Pow(10, 18);
            return price;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting token price for {TokenIn} -> {TokenOut}", tokenIn, tokenOut);
            return null;
        }
    }

    private async Task<decimal> GetPairLiquidityAsync(string router, string tokenA, string tokenB, CancellationToken ct)
    {
        try
        {
            var routerContract = web3.Eth.GetContract(RouterAbi, router);
            var factoryFunction = routerContract.GetFunction("factory");

            var factory = await factoryFunction.CallAsync<FactoryAddress>();

            var factoryContract = web3.Eth.GetContract(FactoryAbi, factory.Address);
            var getPairFunction = factoryContract.GetFunction("getPair");

            var pairAddress = await getPairFunction.CallAsync<string>(tokenA, tokenB);

            if (string.IsNullOrEmpty(pairAddress) || pairAddress == "0x0000000000000000000000000000000000000000")
            {
                return 0;
            }

            var pairContract = web3.Eth.GetContract(PairAbi, pairAddress);
            var getReservesFunction = pairContract.GetFunction("getReserves");
            var reserves = await getReservesFunction.CallAsync<ReservesOutput>();

            // Calculate geometric mean of reserves as liquidity measure
            var reserve0 = (decimal)reserves.Reserve0 / (decimal)Math.Pow(10, 18);
            var reserve1 = (decimal)reserves.Reserve1 / (decimal)Math.Pow(10, 18);

            return (decimal)Math.Sqrt((double)(reserve0 * reserve1));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting pair liquidity for {TokenA}/{TokenB}", tokenA, tokenB);
            return 0;
        }
    }

    [FunctionOutput]
    public class FactoryAddress
    {
        [Parameter("address", "", 1)] public string Address { get; set; }
    }

    [FunctionOutput]
    public class ReservesOutput : IFunctionOutputDTO
    {
        [Parameter("uint112", "reserve0", 1)] public BigInteger Reserve0 { get; set; }
        [Parameter("uint112", "reserve1", 2)] public BigInteger Reserve1 { get; set; }

        [Parameter("uint32", "blockTimestampLast", 3)]
        public uint BlockTimestampLast { get; set; }
    }
}