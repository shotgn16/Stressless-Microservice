using Microsoft.AspNetCore.Mvc.Diagnostics;
using Stressless_Service.Database;
using Stressless_Service.Models;

namespace Stressless_Service.Forecaster
{
    public class EventCalculator
    {
        private async Task<List<CalendarEvents>> FormatEventData(CalenderModel[] calenderEvents)
        {
            List<CalenderModel> events = calenderEvents.ToList();
            List<CalendarEvents> eventRuntime = new List<CalendarEvents>(); 

            if (events.Count > 0) 
            {
                foreach (var item in events)
                {
                    eventRuntime.Add(new CalendarEvents
                    {
                        Runtime = item.StartDate.TimeOfDay - item.EndDate.TimeOfDay,
                        Event = item.StartDate.Date
                    });

                    // EXAMPLE
                    // ======= 
                    // Runtime = 00:05:37
                    // Event: 06/12/2023
                }
            }

            return eventRuntime;
        }

        private async Task<TimeSpan> CalculateFeeTime(List<CalendarEvents> eventRuntimes, ConfigurationModel Configuration)
        {
            TimeSpan TotalRuntime = new TimeSpan();
            TimeSpan FreeTime = new TimeSpan();

            TimeSpan WorkingTime = Configuration.StartTime - Configuration.EndTime;

            if (eventRuntimes.Count > 0)
            {
                // Will iterate through the events in the list. - If they are of the current date, their runtime will be added to the calculation.
                foreach (var ev in eventRuntimes)
                {
                    if (ev.Event.Date == DateTime.Now.Date)
                    {
                        TotalRuntime += ev.Runtime;
                    }
                }
            }

            return FreeTime = WorkingTime - TotalRuntime;
        }

        private async Task StoreDays(List<CalendarEvents> eventRuntimes)
        {
            using (database database = new database())
            {
                foreach (CalendarEvents calendarEvent in eventRuntimes)
                {
                    switch (calendarEvent.Event.DayOfWeek)
                    {
                        case DayOfWeek.Monday:

                            break;
                        case DayOfWeek.Tuesday:

                            break;
                        case DayOfWeek.Wednesday:

                            break;
                        case DayOfWeek.Thursday:

                            break;
                        case DayOfWeek.Friday:

                            break;
                        case DayOfWeek.Saturday:

                            break;
                        case DayOfWeek.Sunday:

                            break;
                    }
                }
            }

        }
    }
}
