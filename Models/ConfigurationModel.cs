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
        public TimeOnly StartTime { get; set; } // WAS (string), 'day_Start'
        public TimeOnly EndTime { get; set; } // WAS: (string), 'day_End'
        public string calenderImport { get; set; }
        public CalenderModel[] calender { get; set; }
    }

    public class CalenderModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly EventDate { get; set; } 
    }
    public class CalendarEvents
    {
        public TimeSpan Runtime { get; set; }
        public DateOnly Event { get; set; }
    }

    public class Reminder
    {
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
    }
}