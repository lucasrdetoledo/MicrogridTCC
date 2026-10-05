using MicrogridTCC.Models;

namespace MicrogridTCC.Services;

/// <summary>
/// Gera um snapshot didático da microgrid fotovoltaica (155 W / 12 V / 7 Ah)
/// sem sensores reais. Os valores seguem um perfil solar ao longo do dia.
/// </summary>
public class MicrogridSimulationService : IMicrogridSimulationService
{
    private const decimal PanelRatedPowerW = 155m;
    private const decimal NominalBusVoltageV = 12m;

    public DashboardViewModel GetSnapshot()
    {
        var now = DateTime.Now;
        var solar = SimulateSolar(now);
        var dcLoad = SimulateDcLoad(now);
        var acLoad = SimulateAcLoad(now);
        var totalLoad = dcLoad + acLoad;
        var netPower = solar.PowerW - totalLoad;
        var isCharging = netPower >= 0;
        var batteryCurrent = Math.Round(Math.Abs(netPower) / NominalBusVoltageV, 2);

        return new DashboardViewModel
        {
            CapturedAt = now,
            IsOnline = true,
            SolarPowerW = solar.PowerW,
            SolarVoltageV = solar.VoltageV,
            SolarCurrentA = solar.CurrentA,
            BatterySocPercent = SimulateSoc(now),
            BatteryVoltageV = SimulateBatteryVoltage(now, isCharging),
            BatteryCurrentA = batteryCurrent,
            IsCharging = isCharging,
            DcLoadW = dcLoad,
            AcLoadW = acLoad,
            History = BuildHistory(now)
        };
    }

    private static (decimal PowerW, decimal VoltageV, decimal CurrentA) SimulateSolar(DateTime instant)
    {
        var irradiance = DaytimeIrradiance(instant);
        var power = Math.Round(PanelRatedPowerW * irradiance, 1);
        var voltage = irradiance < 0.05m
            ? 0m
            : Math.Round(17.4m + (decimal)Math.Sin(instant.Minute / 60.0 * Math.PI) * 0.6m, 1);
        var current = voltage == 0 ? 0m : Math.Round(power / voltage, 2);
        return (power, voltage, current);
    }

    private static decimal DaytimeIrradiance(DateTime instant)
    {
        var hour = instant.Hour + instant.Minute / 60.0;
        if (hour < 6.5 || hour > 18.0)
        {
            return 0m;
        }

        var normalized = (hour - 6.5) / (18.0 - 6.5);
        var bell = Math.Sin(normalized * Math.PI);
        var cloud = 0.88 + 0.12 * Math.Sin(instant.Minute / 7.0);
        return Math.Clamp((decimal)(bell * cloud), 0m, 1m);
    }

    private static decimal SimulateDcLoad(DateTime instant)
    {
        var baseLoad = 12m;
        var extra = instant.Hour is >= 18 and <= 22 ? 8m : 0m;
        return Math.Round(baseLoad + extra + (instant.Minute % 5), 1);
    }

    private static decimal SimulateAcLoad(DateTime instant)
    {
        var baseLoad = 28m;
        var extra = instant.Hour is >= 19 and <= 23 ? 22m : 6m;
        return Math.Round(baseLoad + extra + (instant.Minute % 4) * 0.8m, 1);
    }

    private static decimal SimulateSoc(DateTime instant)
    {
        var irradiance = DaytimeIrradiance(instant);
        var soc = 48m + irradiance * 38m + (decimal)Math.Sin(instant.Hour / 24.0 * Math.PI * 2) * 4m;
        return Math.Clamp(Math.Round(soc, 1), 20m, 100m);
    }

    private static decimal SimulateBatteryVoltage(DateTime instant, bool isCharging)
    {
        var rest = 12.4m + SimulateSoc(instant) / 100m * 0.8m;
        var offset = isCharging ? 0.25m : -0.15m;
        return Math.Round(rest + offset, 2);
    }

    private static List<EnergyHistoryPoint> BuildHistory(DateTime now)
    {
        var points = new List<EnergyHistoryPoint>(24);
        for (var i = 23; i >= 0; i--)
        {
            var instant = now.AddHours(-i);
            var solar = SimulateSolar(instant);
            var consumption = SimulateDcLoad(instant) + SimulateAcLoad(instant);
            points.Add(new EnergyHistoryPoint
            {
                Timestamp = instant,
                SolarPowerW = solar.PowerW,
                ConsumptionW = consumption
            });
        }

        return points;
    }
}
