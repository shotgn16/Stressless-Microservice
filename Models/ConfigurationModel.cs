using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

namespace Stressless_Service.Models
{
    public class ConfigurationModel
    {
        [Column("ConfigurationID")]
        public Guid ID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] WorkingDays { get; set; }
        public TimeOnly DayStartTime { get; set; }
        public TimeOnly DayEndTime { get; set; }
        public string CalenderImport { get; set; }
        [Column(TypeName = "TEXT")]
        public ICollection<CalenderModel> Calender { get; set; }
    }
}