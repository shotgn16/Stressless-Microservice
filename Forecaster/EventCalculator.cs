using Microsoft.AspNetCore.Mvc.Diagnostics;
using ServiceStack;
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

		// Returns a singular 'Timespan' value for the total FreeTime after subtracting event time from thr day
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

		// Will Retrieve all the days of events from the database
		// * If they are bigger than or equal to 21 days stored, then 1 will be deleted until there is enough room to store the new entries
		// * Will only allow 21 days (3 weeks) in the database
		// * will then indert the new days into the database
        public async Task StoreDays(List<CalendarEvents> eventRuntimes)
        {
            List<CalendarEvents[]> OrganisedEvents = new List<CalendarEvents[]>();
            int AlreadyStored = 0;

            using (database database = new database()) {
                List<CalendarEvents> storedDays = await database.GetDays();
                AlreadyStored = storedDays.Count;

                if (AlreadyStored + eventRuntimes.Count >= 21 || AlreadyStored >= 21) {
                    await database.DeleteDays(1); 
                }

                else if (AlreadyStored + eventRuntimes.Count != 21 && AlreadyStored < 21) {
                    eventRuntimes = eventRuntimes.OrderBy(e => e.Event.DayOfWeek).ToList();

                    await database.InsertDay(eventRuntimes);
                }
            }
        }

		// Method to compare runtime and determine free time allocation each day
        public async Task CompareDays()
        {
            List<CalendarEvents> storedDays = new();

            using (database database = new database())
            {
                storedDays = await database.GetDays();
            }


        }
    }
}
