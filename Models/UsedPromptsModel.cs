namespace Stressless_Service.Models
{
    public class UsedPromptsModel
    {
        public int ID { get; set; }
        public int PromptID { get; set; }
        public string LastUsed { get; set; }
    }
}
