namespace MicrogridTCC.Models;

public class DashboardViewModel
{
    public DateTime CapturedAt { get; init; }
    public bool IsOnline { get; init; }

    public decimal SolarPowerW { get; init; }
    public decimal SolarVoltageV { get; init; }
    public decimal SolarCurrentA { get; init; }

    public decimal BatterySocPercent { get; init; }
    public decimal BatteryVoltageV { get; init; }
    public decimal BatteryCurrentA { get; init; }
    public bool IsCharging { get; init; }

    public decimal DcLoadW { get; init; }
    public decimal AcLoadW { get; init; }
    public decimal TotalConsumptionW => DcLoadW + AcLoadW;

    public IReadOnlyList<EnergyHistoryPoint> History { get; init; } = [];

    public string BatteryStatusLabel => IsCharging ? "Carregando" : "Descarregando";
    public string SystemStatusLabel => IsOnline ? "Online" : "Offline";
}
