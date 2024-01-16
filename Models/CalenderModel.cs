using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Stressless_Service.Models
{
    public class CalenderModel : DbContext
    {
        public int CalenderID { get; set; } // Primary Key
        public int ConfigurationID { get; set; } // Foreign Key
        public string Name { get; set; }
        public string Location { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly EventDate { get; set; }
    }
}
