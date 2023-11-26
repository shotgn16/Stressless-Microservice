using System.Timers;
using Stressless_Service.JwtSecurityTokens;

namespace Stressless_Service.Auto_Run
{
    public class AutoBootTimer : IDisposable
    {
        private readonly ILogger<AutoBootTimer> _logger;
        public AutoBootTimer(ILogger<AutoBootTimer> logger) => _logger = logger;

        private static System.Timers.Timer timer;
        private static bool isActive = false;

        public async Task StartABTimer()
        {
            if (!isActive)
            {
                timer = new System.Timers.Timer(60000);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerFinishEvent);
                isActive = true;
                timer.Start();

                Console.WriteLine($"Starting timer on thread: [{Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        private static async void OnTimerFinishEvent(Object source, ElapsedEventArgs e)
        {
            timer.Stop();
            isActive = false;

            Console.WriteLine($"Stopping timer on thread: [{Thread.CurrentThread.ManagedThreadId}]");

            using (AutoBoot autoBoot = new AutoBoot())
                await autoBoot.initializeFront();

            GC.Collect();
        }

        public void Dispose() => GC.Collect();
    }
}
