using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

namespace Stressless_Service.Models
{
    public class ConfigurationModel : DbContext
    {
        public int ConfigurationID { get; set; } // Primary Key
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] WorkingDays { get; set; }
        public TimeOnly DayStartTime { get; set; }
        public TimeOnly DayEndTime { get; set; }
        public string CalenderImport { get; set; }
        public CalenderModel[] Calender { get; set; }

        public static implicit operator ConfigurationModel(List<object> v)
        {
            throw new NotImplementedException();
        }
    }
}