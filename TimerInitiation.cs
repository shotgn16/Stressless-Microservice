using Stressless_Service.Autorun;
using Stressless_Service.Forecaster;
using System.Web.Mvc;

namespace Stressless_Service
{
    public class TimerInitiation : Controller, IDisposable
    {
        private PromptController _promptController;
        private readonly ILogger<TimerInitiation> _logger;
        private BootController _bootController;

        public TimerInitiation(PromptController promptController, ILogger<TimerInitiation> logger, BootController bootController)
        {
            _promptController = promptController;
            _logger = logger;
            _bootController = bootController;
        }

        public async Task InitalizeSystem()
        {
            await _promptController.StartTimer();

            await _bootController.StartTimer();
        }

        public void Dispose() => GC.Collect();
    }
}
