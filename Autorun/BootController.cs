using Microsoft.AspNetCore.Mvc;
using Stressless_Service.Database;
using Stressless_Service.Models;
using System.Diagnostics;
using System.Timers;

namespace Stressless_Service.Autorun
{
    public class BootController : Controller
    {
        private readonly ILogger _logger;
        private readonly IProductRepository _productRepository;

        private static System.Timers.Timer _timer;
        private static DateTime LastSynced;
        private static bool isActive;

        public BootController(ILogger<BootController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<bool> GetSystemTime(bool isWorkingTime = false) // OLD NAME: CheckTime
        {
            if (await _productRepository.CheckConfigurationExists() == 1) {
                ConfigurationClass configuration = await _productRepository.GetConfiguration();

                if (configuration.DayStartTime != TimeOnly.MinValue && configuration.DayEndTime != TimeOnly.MinValue) {
                    DateTime[] Times = await _productRepository.GetShift();

                    if (System.DateTime.Now >= Times[0] && System.DateTime.Now <= Times[1]) {
                        isWorkingTime = true;
                    }
                }
            }

            return isWorkingTime;
        }

        public async Task BootUI() // OLD NAME: initializeFront
        {
            if (await GetSystemTime() && Process.GetProcessesByName("notepad.exe").Length > 0) {
                await StartTimer();
            }

            else {
                if (await LastBooted()) {
                    LastSynced = DateTime.Now;
                    await StartTimer();
                }
            }
        }

        private async Task<bool> LastBooted(bool isLater = false)
        {
            TimeSpan difference = System.DateTime.Now - LastSynced;

            if (difference.TotalHours >= 2) {
                isLater = true;
            }

            return isLater;
        }

        public async Task StartTimer()
        {
            if (!isActive) {
                _timer = new System.Timers.Timer(60000);
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerFinishEvent);
                _timer.Start();

                isActive = true;
                _logger.LogInformation($"Starting timer on thread: [{Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        public void OnTimerFinishEvent(object source, ElapsedEventArgs e)
        {
            _timer.Stop();
            isActive = false;

            TimerRunner();
        }

        private void TimerRunner()
        {
            _logger.LogInformation($"Stopping timer on thread: [{Thread.CurrentThread.ManagedThreadId}]");
            _logger.LogInformation("Starting Stressless Frontend...");

            BootUI();
        }

        public void Dispose() => GC.Collect();
    }
}
