using System.Timers;

namespace Stressless_Service.Forecaster
{
    public interface IPromptController
    {
        void OnTimerFinishEvent(object source, ElapsedEventArgs e);
        Task StartTimer();
    }
}
