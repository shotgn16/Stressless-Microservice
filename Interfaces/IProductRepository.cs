using Stressless_Service.Models;

namespace Stressless_Service.Database
{
    public interface IProductRepository
    {
        ReminderModel GetReminders();
        int CheckConfigurationExists();
        PromptModel GetPrompt(string type);
        List<CalenderEvents> GetDayEvents();
        ConfigurationClass GetConfiguration();
        int GetAuthentication(string macAddress);
        int UserPreviousAuthenticated(string macAddress);
        int UpdateAuthentication(string macAddress);
        UsedPromptsModel GetUsedPrompt(Guid promptid);
        AuthorizeModel GetLatestAuthorization(string macAddress);

        void DeleteExpiredTokens();
        void DeleteConfiguration();
        void DeleteDayEvents(int amount);
        List<string> InsertPrompt(PromptModel prompt);
        void InsertReminders(ReminderModel reminder);
        void InsertUsedPrompt(UsedPromptsModel prompt);
        //Task InsertCalenderEvents(ICollection<CalenderModel> events, Guid ConfigID);
        void InsertDayEvents(List<CalenderEvents> events);
        Guid InsertAuthentication(AuthorizeModel authorize);
        Task<Guid> InsertConfiguration(ConfigurationClass Configuration);
    }
}
