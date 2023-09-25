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
        public string day_Start { get; set; }
        public string day_End { get; set; }
        public string calenderImport { get; set; }
        public CalenderModel[] calender { get; set; }
    }

    public class CalenderModel
    {
        public string name { get; set; }
        public string location { get; set; }
        public string start { get; set; }
        public string finish { get; set; }
    }
}
