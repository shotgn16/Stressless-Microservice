namespace Stressless_Service.Models
{
    public class PromptRequestModel
    {
        public string BulkType { get; set; }
        public List<PromptModel> Prompt { get; set; }
    }
}
