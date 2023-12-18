using Stressless_Service.Auto_Run;
using Stressless_Service.Forecaster;

namespace Stressless_Service
{
    public class timers : IDisposable
    {
        private readonly ILogger logger;

        public async Task InitalizeSystem()
        {

            using (PromptTimer Prompt = new PromptTimer(logger)) {
                await Prompt.StartTimer();
            }

            using (AutoBootTimer Boot = new AutoBootTimer()) {
                await Boot.StartABTimer();
            }
        }

        public void Dispose() => GC.Collect();
    }
}
