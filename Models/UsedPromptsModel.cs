using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Stressless_Service.Models
{
    public class UsedPromptsModel : DbContext
    {
        public int UsedPromptID { get; set; } // Primary Key
        public int PromptID { get; set; } // Foreign Key
        public string LastUsed { get; set; }
    }
}