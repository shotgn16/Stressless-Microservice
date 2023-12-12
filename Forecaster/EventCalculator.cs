using ServiceStack;
using Stressless_Service.Database;
using Stressless_Service.Models;

namespace Stressless_Service.Forecaster
{
    public class EventCalculator : IDisposable
    {
        public async Task EventHandler(CalenderModel[] calenderEvents, ConfigurationModel configuration)
        {
            List<CalendarEvents> events = await FormatEventData(calenderEvents);

            await StoreDays(events);
            await CompareDays(configuration.StartTime, configuration.EndTime);
        }

        // Run Order : 1
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

        // Run Order : 2
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

        // Run Order : 3
        public async Task<TimeSpan> CompareDays(DateTime StartTime, DateTime FinishTime)
        {
            List<CalendarEvents> storedDays = new List<CalendarEvents>();
            TimeSpan WorkTime = StartTime - FinishTime;
            TimeSpan OccupiedTime = new TimeSpan();
            TimeSpan FreeTime = new TimeSpan();

            using (database database = new database()) {
                var days = await database.GetDays();
                
                foreach (var item in days) {
                    if (item.Event == DateTime.Now.Date) {
                        OccupiedTime.Add(item.Runtime);
                        storedDays.Add(item);
                    }
                }

                FreeTime = WorkTime - OccupiedTime;
            }

            return FreeTime;
        }

        // True: Generate Notification | False: Don't do anything...
        public async Task<bool> PromptBreak()
        {
            Reminder LatestReminders = new();
            bool remindUser = false;

            using (database database = new database())
            {
                LatestReminders = await database.GetReminders();

                if (LatestReminders.Date == DateTime.Now.Date && 
                    LatestReminders.Time >= DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromHours(2))) 
                {    
                    remindUser = true;   
                    await database.InsertReminder(new Reminder { Date = DateTime.Now.Date, Time = DateTime.Now.TimeOfDay });
                }
            }

            return remindUser;
        } 

        public void Dispose() => GC.Collect();

        // Run Order : #
        //private async Task<TimeSpan> CalculateFeeTime(List<CalendarEvents> eventRuntimes, ConfigurationModel Configuration)
        //{
        //    TimeSpan TotalRuntime = new TimeSpan();
        //    TimeSpan FreeTime = new TimeSpan();

        //    TimeSpan WorkingTime = Configuration.StartTime - Configuration.EndTime;

        //    if (eventRuntimes.Count > 0)
        //    {
        //        // Will iterate through the events in the list. - If they are of the current date, their runtime will be added to the calculation.
        //        foreach (var ev in eventRuntimes)
        //        {
        //            if (ev.Event.Date == DateTime.Now.Date)
        //            {
        //                TotalRuntime += ev.Runtime;
        //            }
        //        }
        //    }

        //    return FreeTime = WorkingTime - TotalRuntime;
        //}
    }
}
