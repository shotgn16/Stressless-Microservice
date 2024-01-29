using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stressless_Service.Models
{
    public class CalenderEvents
    {
        [Column("EventID")]
        public Guid ID { get; set; }

        public TimeOnly Start { get; set; }
        public TimeOnly Finish {  get; set; }
        public DateOnly Date { get; set; }
    }
}
