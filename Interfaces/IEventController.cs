using Stressless_Service.Models;

namespace Stressless_Service.Forecaster
{
    public interface IEventController
    {
        Task<(TimeSpan, List<CalenderEvents>)> CompareDays(TimeOnly StartTime, TimeOnly FinishTime);
        Task EventHandler(CalenderModel[] calenderEvents, ConfigurationClass configuration);
        Task<List<CalenderEvents>> FormatEventData(CalenderModel[] calenderEvents);
        Task StoreDays(List<CalenderEvents> FormattedEvents);
        Task<bool> PromptBreak();
    }
}
