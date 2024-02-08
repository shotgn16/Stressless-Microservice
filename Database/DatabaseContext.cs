using Microsoft.EntityFrameworkCore;
using Stressless_Service.Models;

namespace Stressless_Service.Database
{
    public class DatabaseContext : DbContext, IDisposable
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> contextOptions)
            : base(contextOptions)
        {
            Database.AutoTransactionBehavior = AutoTransactionBehavior.WhenNeeded;
        }

        public DbSet<UsedPromptsModel> UsedPrompts { get; set; }
        public DbSet<ReminderModel> Reminders { get; set; }
        public DbSet<PromptModel> Prompts { get; set; }
        public DbSet<Configuration_Model> Configuration { get; set; }
        public DbSet<CalenderEvents> CalenderEvents { get; set; }
        public DbSet<AuthorizeModel> Authorize { get; set; }
        public DbSet<CalenderModel> Calender { get; set; }
        public DbSet<AuthenticationTokenModel> Token { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Configuration_Model>().HasKey(pk => pk.ID);
            modelBuilder.Entity<AuthorizeModel>().HasKey(pk => pk.ID);

            modelBuilder.Entity<CalenderEvents>().HasKey(pk => pk.ID);
            modelBuilder.Entity<PromptModel>().HasKey(pk => pk.ID);

            modelBuilder.Entity<UsedPromptsModel>().HasKey(pk => pk.ID);
            modelBuilder.Entity<ReminderModel>().HasKey(pk => pk.ID);
        }

        public void Dispose() => GC.Collect();
    }
}
