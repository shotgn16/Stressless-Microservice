using Stressless_Service.Models;

namespace Stressless_Service.Database
{
    public interface IDatabase
    {
        Task<ConfigurationModel> GetConfiguration();
        Task<int> ConfigurationExists();
        Task DeleteConfiguration();
        Task InsertConfiguration(ConfigurationModel Configuration);
        Task<PromptModel> GetPrompt(string type);
        Task InsertPrompt(PromptModel Prompt);
        Task<UsedPromptsModel> GetUsedPrompts(int PromptID);
        Task InsertUsedPrompt(UsedPromptsModel UsedPrompt);
        Task InsertAuth(AuthorizeModel Authentication);
        Task<int> GetAuth(string MACAddress);
        Task<int> UpdateAuth(string MACAddress);
        Task<DateTime[]> GetShift();
        Task DeleteExpiredTokens();
        Task InsertCalenderEvents(CalenderModel[] calendarEvents);
        Task InsertDay(List<CalenderEvents> Events);
        Task<List<CalenderEvents>> GetDays();
        Task DeleteDays(int amount);
        Task InsertReminder(Reminder Reminder);
        Task<Reminder> GetReminders();
        void Dispose() => GC.Collect();
    }
}
