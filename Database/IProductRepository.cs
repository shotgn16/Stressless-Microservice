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
        Task<UsedPromptsModel> GetUsedPrompt(Guid promptid);
        Task<AuthorizeModel> GetLatestAuthorization(string macAddress);

        Task DeleteExpiredTokens();
        Task DeleteConfiguration();
        void DeleteDayEvents(int amount);
        Task InsertPrompt(PromptModel prompt);
        void InsertReminders(ReminderModel reminder);
        void InsertUsedPrompt(UsedPromptsModel prompt);
        Task InsertCalenderEvents(ICollection<CalenderModel> events);
        Task InsertDayEvents(List<CalenderEvents> events);
        Task InsertAuthentication(AuthorizeModel authorize);
        Task InsertConfiguration(ConfigurationModel Configuration);
        //Task InsertCalenderEvents(CalenderModel[] calender);
    }
}
