namespace dex.monitor.Business.Services.SpreadDetector.Models;

public class SpreadResponse
{
    public SpreadPair ExchangeOne { get; set; }
    public SpreadPair ExchangeTwo { get; set; }
    public decimal AbsoluteSpread { get; set; }
    public decimal RelativeSpread { get; set; }
}

public class SpreadPair
{
    public decimal Rate { get; set; }
    public string Base { get; set; }
    public string Quoted { get; set; }
    public string Exchange { get; set; }
    public string Network { get; set; }
}