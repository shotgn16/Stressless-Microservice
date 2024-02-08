using System.Timers;

namespace Stressless_Service.Autorun
{
    public interface IBootController
    {
        void OnTimerFinishEvent(object source, ElapsedEventArgs e);
        Task<bool> GetSystemTime(bool isWorkingTime = false);
        Task<bool> LastBooted(bool isLater = false);
        void TimerRunner();
        Task StartTimer();
        Task BootUI();
    }
}
