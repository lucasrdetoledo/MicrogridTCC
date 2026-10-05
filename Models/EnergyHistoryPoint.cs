namespace MicrogridTCC.Models;

public class EnergyHistoryPoint
{
    public DateTime Timestamp { get; init; }
    public decimal SolarPowerW { get; init; }
    public decimal ConsumptionW { get; init; }
}
