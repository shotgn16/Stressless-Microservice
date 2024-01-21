using Microsoft.EntityFrameworkCore;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stressless_Service.Models
{
    public class CalenderModel
    {
        [Column("CalenderID")]
        public Guid ID { get; set; }

        public string Name { get; set; }
        public string Location { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly EventDate { get; set; }
    }
}
