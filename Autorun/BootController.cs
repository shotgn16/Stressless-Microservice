using Microsoft.AspNetCore.Mvc;
using Stressless_Service.Database;
using Stressless_Service.Models;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace Stressless_Service.Autorun
{
    public class BootController : Controller, IBootController
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

        // IsUserShift
        public async Task<bool> GetSystemTime(bool isWorkingTime = false)
        {
            int configurationCount = _productRepository.CheckConfigurationExists();

            try
            {
                if (configurationCount == 1)
                {
                    ConfigurationClass configuration = _productRepository.GetConfiguration();

                    if (configuration.DayStartTime != TimeOnly.MinValue && configuration.DayEndTime != TimeOnly.MinValue)
                    {
                        DateTime[] Times = new DateTime[]
                        {
                        Convert.ToDateTime(configuration.DayStartTime),
                        Convert.ToDateTime(configuration.DayEndTime)
                        };

                        if (System.DateTime.Now >= Times[0] && System.DateTime.Now <= Times[1])
                        {
                            isWorkingTime = true;
                        }
                    }
                }

                _logger.LogInformation("Class: 'BootController' | Function: 'IsUserShift' | Status: " + isWorkingTime);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return isWorkingTime;
        }


        public async Task BootUI()
        {
            bool isWorkingHours = await GetSystemTime();
            bool isRecentlyBooted = await LastBooted();

            try
            {
                // If the current system time is within the working hours & the 'StresslessUI' is found in the Task-Manager
                if (isWorkingHours == true && Process.GetProcessesByName("StresslessUI.exe").Length > 0)
                {
                    // Start timer regardless of whether it boots or not
                    await StartTimer();
                }

                else if (isWorkingHours == true)
                {
                    if (isRecentlyBooted == false)
                    {
                        ConfigurationClass Configuration = _productRepository.GetConfiguration();

                        if (Directory.Exists(Configuration.UiLoc))
                        {
                            Process.Start(Configuration.UiLoc + "\\StressLess-Frontend.exe");
                        }

                        LastSynced = DateTime.Now;
                        await StartTimer();
                    }
                }

                _logger.LogInformation("Class: 'BootController' | Function: 'BootUI' | WasRecentlyBooted: " + isRecentlyBooted);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        /// <summary>
        /// * Checks to see if the last time the UI was booted was more than 2 hours ago. If so...
        /// </summary>
        /// <param name="isLater"></param>
        /// <returns></returns>
        public async Task<bool> LastBooted(bool isLater = false)
        {
            TimeSpan difference = System.DateTime.Now - LastSynced;

            if (difference.TotalHours >= 2)
            {
                isLater = true;
            }

            return isLater;
        }

        public async Task StartTimer()
        {
            try
            {
                if (!isActive)
                {
                    _timer = new System.Timers.Timer(60000);
                    _timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerFinishEvent);
                    _timer.Start();

                    isActive = true;
                    _logger.LogInformation($"Class: 'BootController' | Function: 'StartTime' | ID: " + Thread.CurrentThread.ManagedThreadId);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public void OnTimerFinishEvent(object source, ElapsedEventArgs e)
        {
            _timer.Stop();
            isActive = false;

            TimerRunner();
        }

        public void TimerRunner()
        {
            _logger.LogInformation($"Stopping timer on thread: [{Thread.CurrentThread.ManagedThreadId}]");
            _logger.LogInformation("Starting Stressless Frontend...");

            BootUI();
        }
    }
}
