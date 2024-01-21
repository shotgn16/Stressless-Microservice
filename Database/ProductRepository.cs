﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog.Fluent;
using ServiceStack;
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

            _context.Database.EnsureCreated();
        }

        public async Task<ConfigurationModel> GetConfiguration()
        {
            ConfigurationModel TR_Config = new();

            try
            {
                TR_Config = await _context.Configuration
                    .FirstOrDefaultAsync();

                _context.SaveChanges();
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

        public async Task InsertConfiguration(ConfigurationModel Configuration)
        {
            try
            {
                await _context.Configuration.AddAsync(Configuration);
                    _context.SaveChanges();
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
                Response = await _context.UsedPrompts
                    .Where(p => p.ID == PromptID)
                    .OrderByDescending(p => p.LastUsed)
                    .Take(1)
                    .FirstOrDefaultAsync();

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

        public async Task InsertAuthentication(AuthorizeModel Authentication)
        {
            try
            {
                await _context.Authorize.AddAsync(new AuthorizeModel
                {
                    MACAddress = Authentication.MACAddress,
                    LatestLogin = DateTime.Now.ToString(),
                    ClientID = Authentication.ClientID
                });

                _context.SaveChanges();
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

                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return times;
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

        public async Task InsertCalenderEvents(ICollection<CalenderModel> calendarEvents)
        {
            List<CalenderModel> Events = new List<CalenderModel>();

            try
            {
                await _context.Configuration.AddRangeAsync(new ConfigurationModel {
                    Calender = calendarEvents
                });

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
