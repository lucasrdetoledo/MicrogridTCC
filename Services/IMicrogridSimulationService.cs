using MicrogridTCC.Models;

namespace MicrogridTCC.Services;

public interface IMicrogridSimulationService
{
    DashboardViewModel GetSnapshot();
}
