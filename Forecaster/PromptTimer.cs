using NLog.Fluent;
using System.Timers;

namespace Stressless_Service.Forecaster
{
    public class PromptTimer
    {
        private static System.Timers.Timer prTimer;
        private static bool isActive = false;

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

            using EventCalculator Calculator = new EventCalculator(); {
                await Calculator.PromptBreak();
            }

            GC.Collect();
        }

    }
}
