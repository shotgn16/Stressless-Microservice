using Stressless_Service.Models;
using System.Data.Entity;

namespace Stressless_Service.Database
{
    public interface IDatabaseContext : IDisposable
    {
        DbSet<UsedPromptsModel> UsedPrompts { get; set; }
        DbSet<ReminderModel> Reminders { get; set; }
        DbSet<PromptModel> Prompts { get; set; }
        DbSet<Configuration_Model> Configuration { get; set; }
        DbSet<CalenderEvents> CalenderEvents { get; set; }
        DbSet<AuthorizeModel> Authorize { get; set; }
        DbSet<CalenderModel> Calender { get; set; }
        DbSet<AuthenticationTokenModel> Token { get; set; }

        int SaveChanges();
    }

}
