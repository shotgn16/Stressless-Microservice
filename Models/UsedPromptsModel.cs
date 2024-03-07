using System.ComponentModel.DataAnnotations.Schema;

namespace Stressless_Service.Models
{
    public class UsedPromptsModel
    {
        [Column("UsedPromptID")]
        public Guid ID { get; set; }

        public Guid PromptIdentification { get; set; } // Foreign Key
        public string LastUsed { get; set; }
    }
}