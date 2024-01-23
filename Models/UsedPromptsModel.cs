using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using ForeignKeyAttribute = System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute;

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