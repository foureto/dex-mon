using dex.monitor.Business.Services.SpreadDetector.Models;

namespace dex.monitor.Business.Services.SpreadDetector;

internal class SimpleSpreadDetector : ISpreadDetector
{
    public List<SpreadResponse> DetectSpread(List<SpreadPair> spreadPairs)
    {
        var cexPairs = new List<SpreadPair>();
        var dexPairs = new List<SpreadPair>();

        foreach (var spreadPair in spreadPairs)
        {
            if (string.IsNullOrWhiteSpace(spreadPair.Network))
            {
                cexPairs.Add(spreadPair);
                continue;
            }

            dexPairs.Add(spreadPair);
        }

        var result = new List<SpreadResponse>();
        foreach (var dexPair in dexPairs)
        foreach (var cexPair in cexPairs.Where(e => e.Base == dexPair.Base && e.Quoted == dexPair.Quoted))
        {
            var spread = CalculateSpreadBetweenPairs(dexPair, cexPair);
            if (spread != null) result.Add(spread);
        }

        return result;
    }

    private static SpreadResponse CalculateSpreadBetweenPairs(SpreadPair one, SpreadPair two)
    {
        if (one.Rate <= 0 || two.Rate <= 0) return null;
        var absoluteSpread = Math.Abs(one.Rate - two.Rate);
        var relativeSpread = Math.Round(absoluteSpread / Math.Min(one.Rate, two.Rate) * 100, 4);

        return new SpreadResponse
        {
            ExchangeOne = one,
            ExchangeTwo = two,
            AbsoluteSpread = Math.Round(absoluteSpread, 8),
            RelativeSpread = relativeSpread
        };
    }
}