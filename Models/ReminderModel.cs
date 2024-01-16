using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Stressless_Service.Models
{
    public class ReminderModel : DbContext
    {
        public int ReminderID { get; set; } // Primary Key
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
    }
}
