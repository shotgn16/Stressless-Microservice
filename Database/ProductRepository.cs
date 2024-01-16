using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog.Fluent;
using ServiceStack;
using Stressless_Service.Database_EFCore;
using Stressless_Service.Models;
using System.Data.Entity;
using System.Data.SQLite;
using System.Net.Mail;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Stressless_Service.Database
{
    public class ProductRepository : IProductRepository
    {
        private DatabaseContext _context;
        private ILogger<ProductRepository> _logger;

        public ProductRepository(DatabaseContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ConfigurationModel> GetConfiguration()
        {
            ConfigurationModel TR_Config = new();

            try
            {
                TR_Config = await _context.Configuration.
                    FirstOrDefaultAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return TR_Config;
        }

        public async Task<int> CheckConfigurationExists()
        {
            int result = 0;

            try
            {
                result = _context.Configuration
                    .Where(e => e.ConfigurationID == 1)
                    .Count();
            }

            catch (Exception ex)
            {
                _logger.LogError("");
            }

            return result;
        }

        public async Task DeleteConfiguration()
        {
            try
            {
                _context.Configuration.Remove(
                    await _context.Configuration
                        .Where(e => e.ConfigurationID == 1).SingleOrDefaultAsync());
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task InsertConfiguration(ConfigurationModel Configuration)
        {
            try
            {
                await _context.Configuration.AddAsync(Configuration);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<PromptModel> GetPrompt(string type)
        {
            PromptModel Response = new PromptModel();

            try
            {
                var prompt = _context.Prompts
                    .Where(p => p.Type == type)
                    .OrderBy(p => Guid.NewGuid())
                    .Take(1)
                    .FirstOrDefaultAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Response;
        }

        public void InsertPrompt(PromptModel Prompt)
        {
            try
            {
                _context.Prompts.AddAsync(Prompt);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<UsedPromptsModel> GetUsedPrompt(int PromptID)
        {
            UsedPromptsModel Response = new UsedPromptsModel();

            try
            {
                Response = await _context.UsedPrompts
                    .Where(p => p.PromptID == PromptID)
                    .OrderByDescending(p => p.LastUsed)
                    .Take(1)
                    .FirstOrDefaultAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Response;
        }

        public void InsertUsedPrompt(UsedPromptsModel UsedPrompt)
        {
            try
            {
                _context.UsedPrompts.AddAsync(UsedPrompt);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public void InsertAuthentication(AuthorizeModel Authentication)
        {
            try
            {
                _context.Authorize.AddAsync(new AuthorizeModel
                {
                    AuthorizeID = 0,
                    MACAddress = Authentication.MACAddress,
                    LatestLogin = DateTime.Now.ToString(),
                    ClientID = Authentication.ClientID
                });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<int> UserPreviousAuthenticated(string MACAddress)
        {
            int Exists = 0;

            try
            {
                Exists = _context.Authorize
                    .Where(e => e.MACAddress == MACAddress)
                    .SelectMany(e => e.MACAddress)
                    .Count();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Exists;
        }

        public async Task<AuthorizeModel> GetLatestAuthorization(string macAddress)
        {
            AuthorizeModel Authorization = new();

            try
            {
                Authorization = await _context.Authorize
                    .Where(e => e.MACAddress == macAddress)
                        .FirstAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Authorization;
        }

        public async Task<int> GetAuthentication(string macAddress)
        {
            int exists = 0;

            try
            {
                exists = _context.Authorize
                    .Where(e => e.MACAddress == macAddress)
                    .SelectMany(e => e.MACAddress)
                    .Count();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return exists;
        }

        public async Task<int> UpdateAuthentication(string MACAddress)
        {
            int RowsAffected = 0;

            try
            {
                if (MACAddress != null) {
                    var updateAuth = _context.Authorize
                        .Where(e => e.MACAddress == MACAddress)
                            .FirstOrDefault();

                    updateAuth.LatestLogin = DateTime.Now.ToString();
                    RowsAffected = 1;

                    await _context.SaveChangesAsync();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return RowsAffected;
        }

        public async Task<DateTime[]> GetShift()
        {
            DateTime[] times = new DateTime[] { DateTime.Now };

            try
            {
                var configuration = _context.Configuration.Single();

                if (string.IsNullOrEmpty(configuration.DayStartTime.ToString()) || string.IsNullOrEmpty(configuration.DayEndTime.ToString())) {
                    throw new ArgumentNullException("Invalid value detected!");
                }

                else {
                    times = new DateTime[]
                    {
                        Convert.ToDateTime(configuration.DayStartTime),
                        Convert.ToDateTime(configuration.DayEndTime)
                    };
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return times;
        }

        public void DeleteExpiredTokens()
        {
            try
            {
                _context.Authorize.RemoveRange(
                    _context.Authorize.
                        Where(a => a.Expires <= DateTime.Now.AddDays(-1)));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public void InsertCalenderEvents(CalenderModel[] calendarEvents)
        {
            List<CalenderModel> Events = new List<CalenderModel>();

            try
            {
                _context.Calender.AddRangeAsync(calendarEvents);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public void InsertDayEvents(List<CalenderEvents> Events)
        {
            try
            {
                _context.CalenderEvents.AddRangeAsync(Events);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<List<CalenderEvents>> GetDayEvents()
        {
            List<CalenderEvents> Events = new List<CalenderEvents>();

            try
            {
                Events = await _context.CalenderEvents.ToListAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Events;
        }

        public void DeleteDayEvents(int amount)
        {
            try
            {
                _context.CalenderEvents.RemoveRange(_context.CalenderEvents
                    .OrderBy(e => e.Event)
                    .Take(amount));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public void InsertReminders(ReminderModel Reminder)
        {
            try
            {
                _context.Reminders.AddAsync(Reminder);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<ReminderModel> GetReminders()
        {
            ReminderModel reminder = new();

            try
            {
                reminder = await _context.Reminders
                    .OrderByDescending(e => e.Date)
                    .FirstAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return reminder;
        }

        public void Dispose() => GC.Collect();
    }
}
