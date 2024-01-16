using Microsoft.EntityFrameworkCore;
using Stressless_Service.Models;
using System.Configuration;
using System.Reflection;

namespace Stressless_Service.Database_EFCore
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UsedPromptsModel> UsedPrompts { get; set; }
        public DbSet<ReminderModel> Reminders { get; set; }
        public DbSet<PromptModel> Prompts { get; set; }
        public DbSet<ConfigurationModel> Configuration { get; set; }
        public DbSet<CalenderModel> Calender { get; set; }
        public DbSet<CalenderEvents> CalenderEvents { get; set; }
        public DbSet<AuthorizeModel> Authorize { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@$"Data Source={AppDomain.CurrentDomain.BaseDirectory}\\stressless-db.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsedPromptsModel>()
                .HasKey("UsedPromptID");

            modelBuilder.Entity<ReminderModel>()
                .HasKey("ReminderID");

            modelBuilder.Entity<AuthorizeModel>()
                .HasKey("AuthorizeID");

            modelBuilder.Entity<CalenderEvents>()
                .HasKey("EventID");

            modelBuilder.Entity<CalenderModel>()
                .HasKey("CalenderID");

            modelBuilder.Entity<PromptModel>()
                .HasKey("PromptID");

            modelBuilder.Entity<ConfigurationModel>()
                .HasKey("ConfigurationID");
        }
    }
}
