 using System.Globalization;
using System.Text.Json.Nodes;

namespace Stressless_Service.Models
{
    public class ConfigurationModel
    {
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string[] workingDays { get; set; }
        public DateTime StartTime { get; set; } // WAS (string), 'day_Start'
        public DateTime EndTime { get; set; } // WAS: (string), 'day_End'
        public string calenderImport { get; set; }
        public CalenderModel[] calender { get; set; }
    }

    public class CalenderModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CalendarEvents
    {
        public TimeSpan Runtime { get; set; }
        public DateTime Event { get; set; }
    }

    public class Reminder
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
