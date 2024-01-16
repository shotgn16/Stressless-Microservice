using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Stressless_Service.Models
{
    public class CalenderEvents : DbContext
    {
        public int EventID { get; set; } // Primary Key
        public TimeSpan Runtime { get; set; }
        public DateOnly Event { get; set; }
    }
}
