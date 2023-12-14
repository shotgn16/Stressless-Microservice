using ServiceStack;
using Stressless_Service.Database;
using Stressless_Service.Models;

namespace Stressless_Service.Forecaster
{
    public class EventCalculator : IDisposable
    {
        private static TimeSpan FreeTime;

        public async Task EventHandler(CalenderModel[] calenderEvents, ConfigurationModel configuration)
        {
            List<CalendarEvents> events = await FormatEventData(calenderEvents);

            await StoreDays(events);
            (TimeSpan, List<CalendarEvents>) Comparison = await CompareDays(configuration.StartTime, configuration.EndTime);

            FreeTime = Comparison.Item1;
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

        public async Task<(TimeSpan, List<CalendarEvents>)> CompareDays(DateTime StartTime, DateTime FinishTime)
        {
            // Defines instances of the classes and variables needed for this method
            List<CalendarEvents> storedDays = new();
            TimeSpan OccupiedTime = new();
            TimeSpan FreeTime = new();

            // Works out the total time spend in meetings and events by working out the difference between the start and finish times in a specified working day.
            TimeSpan WorkTime = StartTime - FinishTime;

            // Using a database instance to get a list of all the days stored in the database. 
            // The 'Using' statement makes use of 'Disposable' classes, ensuring that they can be cleaned up after they are finished with (reducing the risk of memory leaks).
            using (database database = new database()) {
                var days = await database.GetDays();
                
                // Foreach event in the list of days returned from the database...
                foreach (var item in days) {

                    // If the event ooccurs today
                    if (item.Event == DateTime.Now.Date) {
                        
                        // Add the runtime of that event to an array
                        OccupiedTime.Add(item.Runtime);

                        // Add the event to the List Events for later use.
                        storedDays.Add(item);
                    }
                }

                FreeTime = WorkTime - OccupiedTime;
            }

            return (FreeTime, storedDays);
        }

        // True: Generate Notification | False: Don't do anything...
        public async Task<bool> PromptBreak(ConfigurationModel config = null)
        {
            Reminder LatestReminders = new();
            bool remindUser = false;

            using (database database = new database())
            {
                LatestReminders = await database.GetReminders();

                // Check if...
                // * Date of the event occurred Today [DateTime.Now.Date]
                // * Time of the event was at least 2 hours ago [DateTime.Now.Subtrace(TimeSpan.FromHours(2))]
                if (LatestReminders.Date == DateTime.Now.Date && LatestReminders.Time >= DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromHours(2)))
                {    
                    // Checks if the 'time of the event' : 'LatestReminder.Time' is within the specific working hours the user specified.  
                    if (LatestReminders.Time >= config.StartTime.TimeOfDay && LatestReminders.Time <= config.EndTime.TimeOfDay)
                    {
                        // Must have at least 30 minutes spare minutes to allow a 15 minuit break...
                        if (FreeTime.Minutes >= 30)
                        {
                            // If all the criteria is met - Will insert the reminder into the database and return 'True'
                            // [remindUser = True] will trigger a response to the client that will cause a user prompt to take a break
                            await database.InsertReminder(new Reminder { Date = DateTime.Now.Date, Time = DateTime.Now.TimeOfDay });
                            remindUser = true;
                        }
                    }
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