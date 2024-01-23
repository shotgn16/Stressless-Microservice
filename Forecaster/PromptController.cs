using Microsoft.AspNetCore.Mvc;
using System.Timers;

namespace Stressless_Service.Forecaster
{
    public class PromptController : Controller
    {
        private ILogger<PromptController> _logger;
        private EventController _eventController;

        private static System.Timers.Timer _timer;
        private static bool isActive;

        public PromptController(ILogger<PromptController> logger, EventController eventController)
        {
            _logger = logger;
            _eventController = eventController;
        }

        public async Task StartTimer()
        {
            if (!isActive)
            {
                _timer = new System.Timers.Timer(300000);
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

            _logger.LogInformation($"Stopping... Reminder Checker Timer | Thread: {Thread.CurrentThread.ManagedThreadId}");

            _eventController.PromptBreak();
        }

        public void Dispose() => GC.Collect();
    }
}
