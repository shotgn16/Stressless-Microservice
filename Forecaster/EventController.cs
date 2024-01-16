using ServiceStack;
using Stressless_Service.Database;
using Stressless_Service.Models;
using System.Data.Entity;

namespace Stressless_Service.Forecaster
{
    public class EventController : IDisposable
    {
        private readonly IProductRepository _productRepository;
        private ILogger<EventController> _logger;
        private static TimeSpan FreeTime;

        public EventController(ILogger<EventController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        // Callable method for running the controller in its intended order
        public async Task EventHandler(CalenderModel[] calenderEvents, ConfigurationModel configuration)
        {
            List<CalenderEvents> events = await FormatEventData(calenderEvents);

            await StoreDays(events);
            (TimeSpan, List<CalenderEvents>) Comparison = await CompareDays(configuration.DayStartTime, configuration.DayEndTime);

            FreeTime = Comparison.Item1;
        }

        // Run Order : 1
        private async Task<List<CalenderEvents>> FormatEventData(CalenderModel[] calenderEvents)
        {
            List<CalenderModel> events = calenderEvents.ToList();
            List<CalenderEvents> eventRuntime = new List<CalenderEvents>(); 

            if (events.Count > 0) {
                foreach (var item in events) {
                    eventRuntime.Add(new CalenderEvents
                    {
                        Runtime = item.StartTime - item.EndTime,
                        Event = item.EventDate
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
        public async Task StoreDays(List<CalenderEvents> eventRuntimes)
        {
            int AlreadyStored = 0;

            List<CalenderEvents> storedDays = await _productRepository.GetDayEvents();
            AlreadyStored = storedDays.Count;

            if (AlreadyStored + eventRuntimes.Count >= 21 || AlreadyStored >= 21) {
                _productRepository.DeleteDayEvents(1);
            }

            else if (AlreadyStored + eventRuntimes.Count != 21 && AlreadyStored < 21) {
                eventRuntimes = eventRuntimes.OrderBy(e => e.Event.DayOfWeek).ToList();

                _productRepository.InsertDayEvents(eventRuntimes);
            }
        }

        // Run Order : 3
        public async Task<(TimeSpan, List<CalenderEvents>)> CompareDays(TimeOnly StartTime, TimeOnly FinishTime)
        {
            // Defines instances of the classes and variables needed for this method
            List<CalenderEvents> storedDays = new();
            TimeSpan OccupiedTime = new();
            TimeSpan FreeTime = new();

            // Works out the total time spend in meetings and events by working out the difference between the start and finish times in a specified working day.
            TimeSpan WorkTime = StartTime - FinishTime;

            var days = await _productRepository.GetDayEvents();

            // Foreach event in the list of days returned from the database...
            foreach (var item in days)
            {

                // If the event ooccurs today
                if (item.Event == DateOnly.FromDateTime(DateTime.Now))
                {

                    // Add the runtime of that event to an array
                    OccupiedTime.Add(item.Runtime);

                    // Add the event to the List Events for later use.
                    storedDays.Add(item);
                }
            }

            FreeTime = WorkTime - OccupiedTime;

            return (FreeTime, storedDays);
        }

        // True: Generate Notification | False: Don't do anything...
        public async Task<bool> PromptBreak(ConfigurationModel config = null)
        {
            ReminderModel LatestReminders = new();
            bool remindUser = false;

            LatestReminders = await _productRepository.GetReminders();

            // Check if...
            // * Date of the event occurred Today [DateTime.Now.Date]
            // * Time of the event was at least 2 hours ago [DateTime.Now.Subtrace(TimeSpan.FromHours(2))]
            if (LatestReminders.Date == DateOnly.FromDateTime(DateTime.Now) && LatestReminders.Time >= TimeOnly.FromDateTime(DateTime.Now - TimeSpan.FromHours(2)))
            {
                // Checks if the 'time of the event' : 'LatestReminder.Time' is within the specific working hours the user specified.  
                if (LatestReminders.Time >= config.DayStartTime && LatestReminders.Time <= config.DayEndTime)
                {
                    // Must have at least 30 minutes spare minutes to allow a 15 minuit break...
                    if (FreeTime.Minutes >= 30)
                    {
                        // If all the criteria is met - Will insert the reminder into the database and return 'True'
                        _productRepository.InsertReminders(new ReminderModel { Date = DateOnly.FromDateTime(DateTime.Now), Time = TimeOnly.FromDateTime(DateTime.Now) });

                        // [remindUser = True] will trigger a response to the client that will cause a user prompt to take a break
                        remindUser = true;
                    }
                }
            }

            return remindUser;
        } 

        public void Dispose() => GC.Collect();
    }
}