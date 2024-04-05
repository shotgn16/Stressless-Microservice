using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServiceStack;
using Stressless_Service.Models;
using System.Data.Entity;
using System.Net.Mail;

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

        public ConfigurationClass GetConfiguration()
        {
            Configuration_Model storedConfig = new();
            ConfigurationClass returnedConfig = new();

            try
            {
                storedConfig = _context.Configuration.FirstOrDefault();
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
                _logger.LogError(ex, ex.StackTrace);
            }

            return returnedConfig;
        }

        public int CheckConfigurationExists()
        {
            int result = 0;

            try
            {
                result = _context.Configuration
                    .Where(x => x.FirstName != null)
                    .Count();

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace);
            }

            return result;
        }

        public void DeleteConfiguration()
        {
            try
            {
                //_context.Configuration.Remove(
                //    _context.Configuration
                //        .Where(e => e.FirstName != null)
                //.First());

                _context.Configuration.Remove(
                    _context.Configuration.
                        FirstOrDefault()
                        );

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace);
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
                    UiLoc = Configuration.UiLoc,
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
                _logger.LogError(ex, ex.StackTrace);
            }

            return ConfigurationID;
        }

        public PromptModel GetPrompt(string type)
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
                _logger.LogError(ex, ex.StackTrace);
            }

            return Response;
        }

        public List<string> InsertPrompt(PromptModel Prompt)
        {
            List<string> types = new();

            try
            {
                _context.Prompts.Add(Prompt);
                    _context.SaveChanges();

                types = _context.Prompts
                    .Select(e => e.Type)
                        .Distinct()
                    .ToList();
                
                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace);
            }

            return types;
        }

        public UsedPromptsModel GetUsedPrompt(Guid PromptID)
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
                _logger.LogError(ex, ex.StackTrace);
            }

            return Response;
        }

        public void InsertUsedPrompt(UsedPromptsModel UsedPrompt)
        {
            try
            {
                _context.UsedPrompts.Add(UsedPrompt);
                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace);
            }
        }

        public Guid InsertAuthentication(AuthorizeModel Authentication)
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
                _logger.LogError(ex, ex.StackTrace);
            }

            return AuthID;
        }

        public int UserPreviousAuthenticated(string MACAddress)
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
                _logger.LogError(ex, ex.StackTrace);
            }

            return Exists;
        }

        public AuthorizeModel GetLatestAuthorization(string macAddress)
        {
            AuthorizeModel Authorization = new();

            try
            {
                Authorization = _context.Authorize
                    .Where(e => e.MACAddress == macAddress)
                        .First();

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace);
            }

            return Authorization;
        }

        public int GetAuthentication(string macAddress)
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
                _logger.LogError(ex, ex.StackTrace);
            }

            return exists;
        }

        public int UpdateAuthentication(string MACAddress)
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

                    _context.SaveChanges();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace);
            }

            return RowsAffected;
        }

        public void DeleteExpiredTokens()
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
                _logger.LogError(ex, ex.StackTrace);
            }
        }

        public void InsertDayEvents(List<CalenderEvents> Events)
        {
            try
            {
                _context.CalenderEvents.AddRange(Events);
                    _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace);
            }
        }

        public List<CalenderEvents> GetDayEvents()
        {
            List<CalenderEvents> Events = new List<CalenderEvents>();

            try
            {
                Events = _context.CalenderEvents.ToList();
                    _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace);
            }

            return Events;
        }

        public void DeleteDayEvents(int amount)
        {
            try
            {
                _context.CalenderEvents.RemoveRange(_context.CalenderEvents
                    .OrderBy(e => e.Date)
                    .Take(amount)
                    );

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace);
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
                _logger.LogError(ex, ex.StackTrace);
            }

        }

        public ReminderModel GetReminders()
        {
            ReminderModel reminder = new();

            try
            {
                reminder = _context.Reminders
                    .OrderByDescending(e => e.Date)
                    .First();

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.StackTrace);
            }

            return reminder;
        }
    }
}
