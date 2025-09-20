using dex.monitor.Business.Services.SpreadDetector.Models;

namespace dex.monitor.Business.Services.SpreadDetector;

public interface ISpreadDetector
{
    List<SpreadResponse> DetectSpread(List<SpreadPair> spreadPairs);
}