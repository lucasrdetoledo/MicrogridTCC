using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MicrogridTCC.Models;
using MicrogridTCC.Services;

namespace MicrogridTCC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMicrogridSimulationService _simulationService;

    public HomeController(ILogger<HomeController> logger, IMicrogridSimulationService simulationService)
    {
        _logger = logger;
        _simulationService = simulationService;
    }

    public IActionResult Index()
    {
        var snapshot = _simulationService.GetSnapshot();
        return View(snapshot);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
