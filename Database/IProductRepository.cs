using Stressless_Service.Models;

namespace Stressless_Service.Database
{
    public interface IProductRepository
    {
        Task<DateTime[]> GetShift();
        Task<Reminder> GetReminders();
        Task<int> CheckConfigurationExists();
        Task<PromptModel> GetPrompt(string type);
        Task<List<CalenderEvents>> GetDayEvents();
        Task<ConfigurationModel> GetConfiguration();
        Task<int> GetAuthentication(string macAddress);
        Task<int> UpdateAuthentication(string macAddress);
        Task<UsedPromptsModel> GetUsedPrompt(int promptid);

        void DeleteExpiredTokens();
        void DeleteConfiguration();
        void DeleteDayEvents(int amount);
        void InsertPrompt(PromptModel prompt);
        void InsertReminders(Reminder reminder);
        void InsertUsedPrompt(UsedPromptsModel prompt);
        void InsertCalenderEvents(CalenderModel[] events);
        void InsertDayEvents(List<CalenderEvents> events);
        void InsertAuthentication(AuthorizeModel authorize);
        void InsertConfiguration(ConfigurationModel configuration);
    }
}
