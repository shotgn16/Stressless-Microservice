using Microsoft.EntityFrameworkCore;
using Stressless_Service.Database_EFCore;
using System.ComponentModel.DataAnnotations;

namespace Stressless_Service.Models
{
    public class PromptModel : DbContext
    {
        public int PromptID { get; set; } // Primary Key
        public string Type { get; set; }
        public string Text { get; set; }
    }
}