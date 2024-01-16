using Stressless_Service.Models;

namespace Stressless_Service.Database
{
    public interface IProductRepository
    {
        Task<DateTime[]> GetShift();
        Task<ReminderModel> GetReminders();
        Task<int> CheckConfigurationExists();
        Task<PromptModel> GetPrompt(string type);
        Task<List<CalenderEvents>> GetDayEvents();
        Task<ConfigurationModel> GetConfiguration();
        Task<int> GetAuthentication(string macAddress);
        Task<int> UserPreviousAuthenticated(string macAddress);
        Task<int> UpdateAuthentication(string macAddress);
        Task<UsedPromptsModel> GetUsedPrompt(int promptid);
        Task<AuthorizeModel> GetLatestAuthorization(string macAddress);

        void DeleteExpiredTokens();
        Task DeleteConfiguration();
        void DeleteDayEvents(int amount);
        void InsertPrompt(PromptModel prompt);
        void InsertReminders(ReminderModel reminder);
        void InsertUsedPrompt(UsedPromptsModel prompt);
        void InsertCalenderEvents(CalenderModel[] events);
        void InsertDayEvents(List<CalenderEvents> events);
        void InsertAuthentication(AuthorizeModel authorize);
        Task InsertConfiguration(ConfigurationModel Configuration);
    }
}
