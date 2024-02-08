using Microsoft.AspNetCore.Mvc;
using Stressless_Service.Autorun;
using Stressless_Service.Forecaster;

namespace Stressless_Service
{
    public class TimerInitiation : Controller, ITimeInitiation
    {
        private IPromptController _promptController;
        private readonly ILogger<TimerInitiation> _logger;
        private IBootController _bootController;

        public TimerInitiation(IPromptController promptController, ILogger<TimerInitiation> logger, IBootController bootController)
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
    }
}
