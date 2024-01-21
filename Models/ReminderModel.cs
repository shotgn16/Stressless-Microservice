using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stressless_Service.Models
{
    public class ReminderModel
    {
        [Column("ReminderID")]
        public Guid ID { get; set; }

        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
    }
}
