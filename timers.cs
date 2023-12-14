using Stressless_Service.Auto_Run;
using Stressless_Service.Forecaster;

namespace Stressless_Service
{
    public class timers : IDisposable
    {
        private readonly ILogger logger;

        public async Task InitalizeSystem()
        {

            using (EventCalculator Calculator = new EventCalculator()) {
                await Calculator.PromptBreak();
            }

            using (AutoBootTimer Boot = new AutoBootTimer()) {
                await Boot.StartABTimer();
            }
        }

        public void Dispose() => GC.Collect();
    }
}
