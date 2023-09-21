namespace Stressless_Service.Models
{
    public class PromptRequestModel
    {
        public string BulkType { get; set; }
        public List<PromptModel> Prompt { get; set; }
    }
    public class PromptModel
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
    }
}
