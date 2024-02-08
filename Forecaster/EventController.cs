using Microsoft.AspNetCore.Mvc;
using ServiceStack;
using Stressless_Service.Database;
using Stressless_Service.Models;
using System.Data.Entity;

namespace Stressless_Service.Forecaster
{
    public class EventController : Controller, IEventController
    {
        private readonly IProductRepository _productRepository;
        private ILogger<EventController> _logger;

        private static TimeSpan GBL_FreeTime;
        private static List<CalenderEvents> GBL_Events;

        public EventController(ILogger<EventController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        // Callable method for running the controller in its intended order
        public async Task EventHandler(CalenderModel[] calenderEvents, ConfigurationClass configuration)
        {
            GBL_Events = new List<CalenderEvents>();

            // Formats the calender events into a different Model (CalenderModel => CalenderEvents)
            List<CalenderEvents> FormattedEvents = await FormatEventData(calenderEvents);

            // Stores the 'FormattedEvents [CalenderEvents]' in the database under a 'CalenderEvents' table
            await StoreDays(FormattedEvents);

            (TimeSpan, List<CalenderEvents>) Comparison = await CompareDays(configuration.DayStartTime, configuration.DayEndTime);
                GBL_FreeTime = Comparison.Item1;
                GBL_Events = Comparison.Item2;
        }

        // Run Order : 1
        public async Task<List<CalenderEvents>> FormatEventData(CalenderModel[] calenderEvents)
        {
            List<CalenderModel> events = calenderEvents.ToList();
            List<CalenderEvents> eventRuntime = new List<CalenderEvents>();

            if (events.Count > 0)
            {
                foreach (var item in events)
                {
                    eventRuntime.Add(new CalenderEvents
                    {
                        Start = item.StartTime,
                        Finish = item.EndTime,
                        Date = item.EventDate
                    });

                    // EXAMPLE
                    // =======  
                    // Start = 09:00:00
                    // Finish = 09:30:00
                    // Event: 06/12/2023
                }
            }

            return eventRuntime;
        }

        // Run Order : 2
        public async Task StoreDays(List<CalenderEvents> FormattedEvents)
        {
            List<CalenderEvents> storedDays = await _productRepository.GetDayEvents();
            int CountOfDaysAlreadyStored = storedDays.Count;

            CheckNumberOfDaysInDatabase: // Label
            if (CountOfDaysAlreadyStored + FormattedEvents.Count >= 21)
            {
                _productRepository.DeleteDayEvents(1);
                    goto CheckNumberOfDaysInDatabase; // Goto 'Label'
            }

            else if (CountOfDaysAlreadyStored + FormattedEvents.Count < 21)
            {
                FormattedEvents = FormattedEvents.OrderBy(e => e.Date.DayOfWeek).ToList();
                    await _productRepository.InsertDayEvents(FormattedEvents);
            }
        }

        // Run Order : 3
        public async Task<(TimeSpan, List<CalenderEvents>)> CompareDays(TimeOnly StartTime, TimeOnly FinishTime)
        {
            TimeSpan timeInMeetings = new();
            List<CalenderEvents> storedDays = new();
            TimeSpan workingHours = StartTime - FinishTime;
            List<CalenderEvents> daysStoredInDatabase = await _productRepository.GetDayEvents();

            foreach (var item in daysStoredInDatabase)
            {
                if (item.Date == DateOnly.FromDateTime(DateTime.Today))
                {
                    timeInMeetings.Add(item.Start - item.Finish);
                        storedDays.Add(item);
                }
            }

            // Returns
            //  * Freetime - Total free time left within the day represented by an int value
            //  * storedDays - a List, in the format of the (CalenderEvents) model, of all events from the Configuration that are dated the current date!
            return (workingHours - timeInMeetings, storedDays);
        }

        public async Task<bool> PromptBreak()
        {
            int busyTimeIndicator = 0;
            bool promptUserForABreak = false;
            bool isUserBusy = false;
            ReminderModel latestReminders = await _productRepository.GetReminders();
            ConfigurationClass configuration = await _productRepository.GetConfiguration();
            List<CalenderEvents> todaysEvents = await _productRepository.GetDayEvents();

            if (latestReminders.Time >= TimeOnly.FromDateTime(DateTime.Now + TimeSpan.FromHours(2)) &&
                latestReminders.Time >= configuration.DayStartTime &&
                latestReminders.Time < configuration.DayEndTime)
            {
                todaysEvents = (List<CalenderEvents>)todaysEvents.Where(x => x.Date == DateOnly.FromDateTime(DateTime.Now));
                foreach (var e in todaysEvents)
                {
                    TimeSpan eventDuration = e.Start - e.Finish;

                    if (TimeOnly.FromDateTime(DateTime.Now) == e.Start || TimeOnly.FromDateTime(DateTime.Now) == e.Start.AddMinutes(Convert.ToDouble(eventDuration)))
                    {
                        isUserBusy = true;
                    }
                }

                if (GBL_FreeTime.Minutes >= 20 && isUserBusy == false) // MUST HAVE AT LEAST 20 MINUTES OF FREE TIME PER DAY FOR THIS TO WORK!
                {
                    TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
                    foreach (var item in GBL_Events)
                    {
                        if (currentTime >= item.Start && currentTime <= item.Finish)
                        {
                            busyTimeIndicator++;
                        }
                    }

                    if (busyTimeIndicator == 0)
                    {
                        _productRepository.InsertReminders(
                            new ReminderModel
                            {
                                Date = DateOnly.FromDateTime(DateTime.Now),
                                Time = TimeOnly.FromDateTime(DateTime.Now)
                            });

                        promptUserForABreak = true;
                    }
                }
            }

            return promptUserForABreak;
        }


    }
}