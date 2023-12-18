 using System.Globalization;
using System.Text.Json.Nodes;

namespace Stressless_Service.Models
{
    public class ConfigurationModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] WorkingDays { get; set; }
        public TimeOnly DayStartTime { get; set; }
        public TimeOnly DayEndTime { get; set; }
        public string CalenderImport { get; set; }
        public CalenderModel[] Calender { get; set; }
    }
}