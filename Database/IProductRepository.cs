using Stressless_Service.Models;

namespace Stressless_Service.Database
{
    public interface IProductRepository
    {
        Task<ReminderModel> GetReminders();
        int CheckConfigurationExists();
        Task<PromptModel> GetPrompt(string type);
        Task<List<CalenderEvents>> GetDayEvents();
        Task<ConfigurationClass> GetConfiguration();
        Task<int> GetAuthentication(string macAddress);
        Task<int> UserPreviousAuthenticated(string macAddress);
        Task<int> UpdateAuthentication(string macAddress);
        Task<UsedPromptsModel> GetUsedPrompt(Guid promptid);
        Task<AuthorizeModel> GetLatestAuthorization(string macAddress);

        Task DeleteExpiredTokens();
        Task DeleteConfiguration();
        void DeleteDayEvents(int amount);
        Task<List<string>> InsertPrompt(PromptModel prompt);
        void InsertReminders(ReminderModel reminder);
        void InsertUsedPrompt(UsedPromptsModel prompt);
        //Task InsertCalenderEvents(ICollection<CalenderModel> events, Guid ConfigID);
        Task InsertDayEvents(List<CalenderEvents> events);
        Task<Guid> InsertAuthentication(AuthorizeModel authorize);
        Task<Guid> InsertConfiguration(ConfigurationClass Configuration);
    }
}
