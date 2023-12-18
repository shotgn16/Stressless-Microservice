using NLog.Fluent;
using System.Timers;

namespace Stressless_Service.Forecaster
{
    public class PromptTimer : IDisposable
    {
        private static System.Timers.Timer prTimer;
        private static bool isActive = false;
        private ILogger logger;

        public PromptTimer(ILogger _logger) => logger = _logger; 

        public async Task StartTimer()
        {
            if (!isActive) {
                prTimer = new System.Timers.Timer(300000);
                prTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerFinishEvent);
                
                isActive = true;
                prTimer.Start();

                Log.Info($"Starting... Reminder Checker Timer | Thread: {Thread.CurrentThread.ManagedThreadId}");
            }
        }

        public async void OnTimerFinishEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine($"Stopping... Reminder Checker Timer | Thread: {Thread.CurrentThread.ManagedThreadId}");

            prTimer.Stop();
            isActive = false;

            using EventCalculator Calculator = new EventCalculator(logger); {
                await Calculator.PromptBreak();
            }

            GC.Collect();
        }

        public void Dispose() => GC.Collect();

    }
}