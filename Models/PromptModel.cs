using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stressless_Service.Models
{
    public class PromptModel
    {
        [Column("PromptID")]
        public Guid ID { get; set; }

        public string Type { get; set; }
        public string Text { get; set; }
    }
}