using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Reflection;
using Dapper;
using Newtonsoft.Json;
using Stressless_Service.Factories;
using Stressless_Service.Models;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Stressless_Service.Database
{
    public class database : IDisposable
    {
        private ILogger logger { get; set; }
        private DatabaseFactory _databaseFactory;

        public database(ILogger _logger)
        {
            logger = _logger;

            _databaseFactory = new DatabaseFactory(logger);
            _databaseFactory.databasebBuild($"Data Source={Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\database.db;Version=3;");
        }

        public async Task<ConfigurationModel> GetConfiguration()
        {
            ConfigurationModel TaskResponse_Configuration = new();
            List<CalenderModel> Calenderlist = new();

            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();

                    TaskResponse_Configuration = await Connection.QueryFirstAsync<ConfigurationModel>("SELECT ID, FirstName, LastName, DayStartTime, DayEndTime, CalenderImport FROM Configuration");

                    string strCalender = await Connection.QueryFirstAsync<string>("SELECT Calender FROM Configuration");
                    string strWorkingDays = await Connection.QueryFirstAsync<string>("SELECT WorkingDays FROM Configuration");

                    Calenderlist = JsonConvert.DeserializeObject<List<CalenderModel>>(strCalender);

                    // Assigning the remaining values
                    TaskResponse_Configuration.WorkingDays = JsonConvert.DeserializeObject<string[]>(strWorkingDays);
                    TaskResponse_Configuration.Calender = Calenderlist.ToArray();

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }

            return TaskResponse_Configuration;
        }

        public async Task<int> ConfigurationExists()
        {
            int configExists = 0;

            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();

                    configExists = Connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Configuration WHERE ID = 1;");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }

            return configExists;
        }

        public async Task DeleteConfiguration()
        {
            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();
                    await Connection.ExecuteAsync("DELETE * FROM CONFIGURATION WHERE ID = '1';");
                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        public async Task InsertConfiguration(ConfigurationModel Configuration)
        {
            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();

                    await Connection.ExecuteAsync("INSERT INTO Configuration (ID, FirstName, LastName, WorkingDays, DayStartTime, DayEndTime, CalenderImport, Calender) " +
                        "VALUES (1, '"
                        + Configuration.FirstName + "', '" + Configuration.LastName + "', '" + JsonConvert.SerializeObject(Configuration.WorkingDays) + "', '" + Configuration.DayStartTime + "', '"
                        + Configuration.DayEndTime + "', '" + Configuration.CalenderImport + "', '" + JsonConvert.SerializeObject(Configuration.Calender) + "');");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        public async Task<PromptModel> GetPrompt(string type)
        {
            PromptModel Response = new PromptModel();

            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();
                    Response = Connection.QuerySingle<PromptModel>("SELECT * FROM Prompts WHERE Type = '" + type + "' ORDER BY RANDOM() LIMIT 1;");
                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }

            return Response;
        }

        public async Task InsertPrompt(PromptModel Prompt)
        {
            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();
                    Connection.Execute(
                        "INSERT INTO Prompts (ID, Type, Text) VALUES (" +
                        "'" + string.Empty + "'," +
                        "'" + Prompt.Type + "'," +
                        "'" + Prompt.Text + "');");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        public async Task<UsedPromptsModel> GetUsedPrompts(int PromptID)
        {
            UsedPromptsModel Response = new UsedPromptsModel();

            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();
                    Response = Connection.QuerySingle<UsedPromptsModel>(
                        "SELECT * FROM UsedPrompts WHERE PromptID = '" + PromptID + "' ORDER BY LastUsed DESC;");
                    
                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }

            return Response;
        }

        public async Task InsertUsedPrompt(UsedPromptsModel UsedPrompt)
        {
            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();

                    Connection.Execute(
                        "INSERT INTO UsedPrompts (ID, PromptID, LastUsed) VALUES (" +
                        "'" + string.Empty + "', " +
                        "'" + UsedPrompt.PromptID + "', " +
                        "'" + UsedPrompt.LastUsed + "');");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        public async Task InsertAuth(AuthorizeModel Authentication)
        {
            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();
                    Connection.Execute(
                        "INSERT INTO 'Auth' " +
                        "(ID, MACAddress, DateCreated, ClientID) VALUES (" +
                        "'" + string.Empty + "', " +
                        "'" + Authentication.MACAddress + "', " +
                        "'" + DateTime.Now + "', " +
                        "'" + Authentication.ClientID + "');");
                    
                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        public async Task<int> GetAuth(string MACAddress)
        {
            int Exists = 0;

            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();
                    Exists = Connection.ExecuteScalar<int>("SELECT count(*) FROM Auth WHERE MACAddress = '" + MACAddress + "';");
                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }

            return Exists;
        }

        public async Task<int> UpdateAuth(string MACAddress)
        {
            int RowsAffected = 0;

            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();
                    RowsAffected = Connection.Execute($"UPDATE Auth SET DateCreated = '{DateTime.Now}' WHERE MACAddress = '{MACAddress}';");
                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }

            return RowsAffected;
        }

        public async Task<DateTime[]> GetShift()
        {
            DateTime[] times = new DateTime[] { DateTime.Now };

            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();

                    string StartShift = Connection.QuerySingle<string>("SELECT Start_time, Finish_time FROM Configuration");
                    string FinishShift = Connection.QuerySingle<string>("SELECT Finish_time FROM Configuration");

                    await Connection.CloseAsync();

                    if (string.IsNullOrEmpty(StartShift) || string.IsNullOrEmpty(FinishShift)) {
                        throw new ArgumentNullException("Invalid value detected!"); 
                    }

                    else {
                        times = new DateTime[] { Convert.ToDateTime(StartShift), Convert.ToDateTime(FinishShift) }; 
                    }
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }

            return times;
        }

        public async Task DeleteExpiredTokens()
        {
            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();
                    await Connection.ExecuteAsync("DELETE * FROM Auth WHERE datetime('now', '-1 day') >= Generated;");
                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        public async Task InsertCalenderEvents(CalenderModel[] calendarEvents)
        {
            List<CalenderModel> Events = new List<CalenderModel>();

            try
            {
                Events = calendarEvents.ToList();

                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();

                    foreach (var item in calendarEvents)
                    {
                        await Connection.ExecuteAsync(
                            "INSERT INTO 'Calendar' (ID, Name, Start, Finish) VALUES (" +
                            "'" + string.Empty + "', " +
                            "'" + item.Name + "', " +
                            "'" + item.StartTime + "', " +
                            "'" + item.EndTime + "', " + 
                            "'" + item.EventDate + "');");
                    
                    } await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        public async Task InsertDay(List<CalenderEvents> Events)
        {
            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    foreach (CalenderEvents E in Events)
                    {
                        await Connection.OpenAsync();
                        await Connection.ExecuteAsync("INSERT INTO Events (Runtime, Date) VALUES ('" + E.Runtime + "', '" + E.Event + "');");
                        await Connection.CloseAsync();
                    }
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        public async Task<List<CalenderEvents>> GetDays()
        {
            List<CalenderEvents> Events = new List<CalenderEvents>();

            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();

                    foreach (List<CalenderEvents> eventList in await Connection.QueryAsync<List<CalenderEvents>>("SELECT * FROM Events"))
                    {
                        Events.AddRange(eventList);

                    } await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }

            return Events;
        }

        public async Task DeleteDays(int amount)
        {
            try
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection())
                {
                    await Connection.OpenAsync();
                    await Connection.ExecuteAsync("DELETE FROM Events ORDER BY Date ASC Limit " + amount + ";");
                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }

        public async Task InsertReminder(Reminder Reminder)
        {
            try {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection()) 
                {
                    await Connection.OpenAsync();
                    await Connection.ExecuteAsync("INSERT INTO Reminders (Date, Time) VALUES ('" + Reminder.Date + "', '" + Reminder.Time + "');");
                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex) {
                logger.LogError(ex.Message, ex);
            }
        }

        public async Task<Reminder> GetReminders()
        {
            Reminder reminder = new();

            try 
            {
                using (SQLiteConnection Connection = await _databaseFactory.CreateConnection()) 
                {
                    await Connection.OpenAsync();
                    reminder = await Connection.QuerySingleAsync<Reminder>("SELECT * FROM Reminders ORDER BY LastUsed DESC;");
                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex) {
                logger.LogError(ex.Message, ex);
            }

            return reminder;
        }

        public void Dispose() => GC.Collect();
    }
}