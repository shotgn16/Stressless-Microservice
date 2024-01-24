using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServiceStack;
using Stressless_Service.Models;
using System.Data.Entity;

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

            _context.Database.EnsureCreated();
        }

        public async Task<ConfigurationClass> GetConfiguration()
        {
            Configuration_Model storedConfig = new();
            ConfigurationClass returnedConfig = new();

            try
            {
                storedConfig = await _context.Configuration.FirstOrDefaultAsync();
                _context.SaveChanges();

                returnedConfig = new ConfigurationClass
                {
                    FirstName = storedConfig.FirstName,
                    LastName = storedConfig.LastName,
                    DayStartTime = storedConfig.DayStartTime,
                    DayEndTime = storedConfig.DayEndTime,
                    WorkingDays = storedConfig.WorkingDays,
                    CalenderImport = storedConfig.CalenderImport,
                    Calender = (CalenderModel[])JsonConvert.DeserializeObject(storedConfig.Calender),
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return returnedConfig;
        }

        public async Task<int> CheckConfigurationExists()
        {
            int result = 0;

            try
            {
                result = _context.Configuration.Count();
                _context.SaveChanges();
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
                        .Where(e => e.FirstName != null)
                .FirstOrDefaultAsync());

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        // Everything, Except Calender events!
        public async Task<Guid> InsertConfiguration(ConfigurationClass Configuration)
        {
            Guid ConfigurationID = new();

            try
            {
                await _context.Configuration.AddAsync(new Configuration_Model
                {
                    FirstName = Configuration.FirstName,
                    LastName = Configuration.LastName,
                    WorkingDays = Configuration.WorkingDays,
                    DayStartTime = Configuration.DayStartTime,
                    DayEndTime = Configuration.DayEndTime,
                    CalenderImport = Configuration.CalenderImport,
                    Calender = JsonConvert.SerializeObject(Configuration.Calender)
                });

                _context.SaveChanges();

                ConfigurationID = _context.Configuration
                    .Where(e => e.FirstName == Configuration.FirstName)
                    .Select(a => a.ID).FirstNonDefault();

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return ConfigurationID;
        }

        public async Task<PromptModel> GetPrompt(string type)
        {
            PromptModel Response = new PromptModel();

            try
            {
                var random = new Random();
                var rndIndex = random.Next();

                Response = _context.Prompts
                    .Where(p => p.Type == type)
                    .OrderBy(x => rndIndex)
                    .FirstOrDefault();

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Response;
        }

        public async Task InsertPrompt(PromptModel Prompt)
        {
            try
            {
                await _context.Prompts.AddAsync(Prompt);
                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<UsedPromptsModel> GetUsedPrompt(Guid PromptID)
        {
            UsedPromptsModel Response = new UsedPromptsModel();

            try
            {
                Response = _context.UsedPrompts
                    .Where(p => p.PromptIdentification == PromptID)
                        .OrderByDescending(p => p.LastUsed)
                    .FirstOrDefault();

                _context.SaveChanges();
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
                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<Guid> InsertAuthentication(AuthorizeModel Authentication)
        {
            Guid AuthID = new();

            try
            {
                _context.Authorize.Add(Authentication);
                _context.SaveChanges();

                // Will return the ID Generated by the database
                AuthID = _context.Authorize
                    .Where(e => e.Expires == Authentication.Expires)
                    .Select(p => p.ID).FirstOrDefault();

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return AuthID;
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

                _context.SaveChanges();
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

                _context.SaveChanges();
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

                _context.SaveChanges();
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
                if (MACAddress != null)
                {
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

        public async Task DeleteExpiredTokens()
        {
            try
            {
                _context.Authorize.RemoveRange(
                    _context.Authorize.
                        Where(a => a.Expires <= DateTime.Now.AddDays(-1)));

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task InsertDayEvents(List<CalenderEvents> Events)
        {
            try
            {
                await _context.CalenderEvents.AddRangeAsync(Events);
                _context.SaveChanges();
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
                _context.SaveChanges();
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

                _context.SaveChanges();
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
                _context.SaveChanges();
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

                _context.SaveChanges();
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
