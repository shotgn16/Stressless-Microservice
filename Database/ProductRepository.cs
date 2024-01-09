using Dapper;
using Newtonsoft.Json;
using NLog.Fluent;
using ServiceStack;
using Stressless_Service.Models;
using System.Data.SQLite;

namespace Stressless_Service.Database
{
    public class ProductRepository : IProductRepository
    {
        private readonly DBConnectionFactory _connectionFactory;
        private ILogger<ProductRepository> _logger;

        public ProductRepository(DBConnectionFactory connectionFactory, ILogger<ProductRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<ConfigurationModel> GetConfiguration()
        {
            ConfigurationModel TR_Config = new();
            List<CalenderModel> CalenderList = new();

            try
            {
                using (var conn = _connectionFactory.CreateConnection())
                {
                    conn.Open();

                    TR_Config = await conn.QueryFirstAsync<ConfigurationModel>("SELECT ID, FirstName, LastName, DayStartTime, DayEndTime, CalenderImport FROM Configuration");

                    string strCalender = await conn.QueryFirstAsync<string>("SELECT Calender FROM Configuration");
                    string strWorkingDays = await conn.QueryFirstAsync<string>("SELECT WorkingDays FROM Configuration");

                    CalenderList = JsonConvert.DeserializeObject<List<CalenderModel>>(strCalender);

                    // Assigning the remaining values
                    TR_Config.WorkingDays = JsonConvert.DeserializeObject<string[]>(strWorkingDays);
                    TR_Config.Calender = CalenderList.ToArray();

                    conn.Close();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return TR_Config;
        }

        public async Task<int> CheckConfigurationExists()
        {
            int configexists = 0;

            try
            {
                using (var conn = _connectionFactory.CreateConnection())
                {
                    conn.Open();

                    configexists = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM CONFIGURATION WHERE ID = 1;");

                    conn.Close();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError("");
            }

            return configexists;
        }

        public void DeleteConfiguration()
        {
            try
            {
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();
                    Connection.ExecuteAsync("DELETE * FROM CONFIGURATION WHERE ID = '1';");
                    Connection.Close();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public void InsertConfiguration(ConfigurationModel Configuration)
        {
            try
            {
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();

                    Connection.ExecuteAsync("INSERT INTO Configuration (ID, FirstName, LastName, WorkingDays, DayStartTime, DayEndTime, CalenderImport, Calender) " +
                        "VALUES (1, '"
                        + Configuration.FirstName + "', '" + Configuration.LastName + "', '" + JsonConvert.SerializeObject(Configuration.WorkingDays) + "', '" + Configuration.DayStartTime + "', '"
                        + Configuration.DayEndTime + "', '" + Configuration.CalenderImport + "', '" + JsonConvert.SerializeObject(Configuration.Calender) + "');");

                    Connection.Close();
                }
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
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();
                    Response = Connection.QuerySingle<PromptModel>("SELECT * FROM Prompts WHERE Type = '" + type + "' ORDER BY RANDOM() LIMIT 1;");
                    Connection.Close();
                }
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
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();
                    
                    Connection.Execute(
                        "INSERT INTO Prompts (ID, Type, Text) VALUES (" +
                        "'" + string.Empty + "'," +
                        "'" + Prompt.Type + "'," +
                        "'" + Prompt.Text + "');");

                    Connection.Close();
                }
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
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();
                    Response = Connection.QuerySingle<UsedPromptsModel>(
                        "SELECT * FROM UsedPrompts WHERE PromptID = '" + PromptID + "' ORDER BY LastUsed DESC;");

                    Connection.Close();
                }
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
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();

                    Connection.Execute(
                        "INSERT INTO UsedPrompts (ID, PromptID, LastUsed) VALUES (" +
                        "'" + string.Empty + "', " +
                        "'" + UsedPrompt.PromptID + "', " +
                        "'" + UsedPrompt.LastUsed + "');");

                    Connection.Close();
                }
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
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();
                    
                    Connection.Execute(
                        "INSERT INTO 'Auth' " +
                        "(ID, MACAddress, DateCreated, ClientID) VALUES (" +
                        "'" + string.Empty + "', " +
                        "'" + Authentication.MACAddress + "', " +
                        "'" + DateTime.Now + "', " +
                        "'" + Authentication.ClientID + "');");

                    Connection.Close();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<int> GetAuthentication(string MACAddress)
        {
            int Exists = 0;

            try
            {
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();
                    Exists = Connection.ExecuteScalar<int>("SELECT count(*) FROM Auth WHERE MACAddress = '" + MACAddress + "';");
                    Connection.Close();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Exists;
        }

        public async Task<int> UpdateAuthentication(string MACAddress)
        {
            int RowsAffected = 0;

            try
            {
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();
                    RowsAffected = Connection.Execute($"UPDATE Auth SET DateCreated = '{DateTime.Now}' WHERE MACAddress = '{MACAddress}';");
                    Connection.Close();
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
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();

                    string StartShift = Connection.QuerySingle<string>("SELECT Start_time, Finish_time FROM Configuration");
                    string FinishShift = Connection.QuerySingle<string>("SELECT Finish_time FROM Configuration");

                    Connection.Close();

                    if (string.IsNullOrEmpty(StartShift) || string.IsNullOrEmpty(FinishShift))
                    {
                        throw new ArgumentNullException("Invalid value detected!");
                    }

                    else
                    {
                        times = new DateTime[] { Convert.ToDateTime(StartShift), Convert.ToDateTime(FinishShift) };
                    }
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
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();
                    Connection.ExecuteAsync("DELETE * FROM Auth WHERE datetime('now', '-1 day') >= Generated;");
                    Connection.Close();
                }
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
                Events = calendarEvents.ToList();

                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();

                    foreach (var item in calendarEvents)
                    {
                        Connection.ExecuteAsync(
                            "INSERT INTO 'Calendar' (ID, Name, Start, Finish) VALUES (" +
                            "'" + string.Empty + "', " +
                            "'" + item.Name + "', " +
                            "'" + item.StartTime + "', " +
                            "'" + item.EndTime + "', " +
                            "'" + item.EventDate + "');");

                    }
                    Connection.Close();
                }
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
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    foreach (CalenderEvents E in Events)
                    {
                        Connection.Open();
                        Connection.ExecuteAsync("INSERT INTO Events (Runtime, Date) VALUES ('" + E.Runtime + "', '" + E.Event + "');");
                        Connection.Close();
                    }
                }
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
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();

                    foreach (List<CalenderEvents> eventList in await Connection.QueryAsync<List<CalenderEvents>>("SELECT * FROM Events"))
                    {
                        Events.AddRange(eventList);

                    }
                    Connection.Close();
                }
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
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();
                    Connection.ExecuteAsync("DELETE FROM Events ORDER BY Date ASC Limit " + amount + ";");
                    Connection.Close();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public void InsertReminders(Reminder Reminder)
        {
            try
            {
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();
                    Connection.ExecuteAsync("INSERT INTO Reminders (Date, Time) VALUES ('" + Reminder.Date + "', '" + Reminder.Time + "');");
                    Connection.Close();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<Reminder> GetReminders()
        {
            Reminder reminder = new();

            try
            {
                using (var Connection = _connectionFactory.CreateConnection())
                {
                    Connection.Open();
                    reminder = await Connection.QuerySingleAsync<Reminder>("SELECT * FROM Reminders ORDER BY LastUsed DESC;");
                    Connection.Close();
                }
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
