using System.Collections;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Net;
using System.Reflection;
using System.Text.Json.Nodes;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Fluent;
using NLog.Web;
using Stressless_Service.Forecaster;
using Stressless_Service.JwtSecurityTokens;
using Stressless_Service.Models;

namespace Stressless_Service.Database
{
    public class database : IDisposable
    {
        Logger classLogger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        public database() => dbBuild();
        private string _connectionString { get; set; }

        private async Task<bool> dbBuild()
        {
            bool dbExists = false;
            
            try
            {
                _connectionString = $"Data Source={Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\database.db;Version=3;";

                if (File.Exists($"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\database.db"))
                {
                    dbExists = true;
                    goto End;
                }

                else if (!File.Exists($"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\database.db"))
                {
                    SQLiteConnection.CreateFile($"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\database.db");
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }

        End:
            await dbSetup();
            return dbExists;
        }

        public async Task<SQLiteConnection> CreateConnection()
          {
            SQLiteConnection connection = null;

            try
            {
                connection = new SQLiteConnection(_connectionString);
            }

            catch (Exception ex) 
            {
                classLogger.Error(ex.Message, ex);
            }

            return connection;
        }

        public async Task<Task> dbSetup()
        {
            using (SQLiteConnection connection = await CreateConnection())
            {
                await connection.OpenAsync();

                try
                {
                    int table_Configuration = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Prompts';");
                    if (table_Configuration.Equals(0)) {
                        await connection.ExecuteAsync("CREATE TABLE 'Configuration' ('ID' INTEGER, 'Firstname' TEXT, 'Lastname' INTEGER, 'WorkingDays' TEXT, 'Start_time' TEXT, 'Finish_time' TEXT, 'CalenderImport' TEXT, 'Calender' TEXT, PRIMARY KEY('ID'));");
                    }

                    int table_Prompts = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Prompts';");
                    if (table_Configuration.Equals(0)) {
                        await connection.ExecuteAsync("CREATE TABLE 'Prompts' ('ID' INTEGER, 'Type' TEXT, 'Text' TEXT, PRIMARY KEY('ID'));");
                    }

                    int table_UsedPrompts = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='UsedPrompts';");
                    if (table_UsedPrompts.Equals(0)) {
                        await connection.ExecuteAsync("CREATE TABLE 'UsedPrompts' ('ID' INTEGER, 'PromptID' INTEGER, 'LastUsed' TEXT, FOREIGN KEY('PromptID') REFERENCES 'Prompts', PRIMARY KEY('ID'));");
                    }

                    int table_Auth = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Auth';");
                    if (table_Auth.Equals(0)) {
                        await connection.ExecuteAsync("CREATE TABLE 'Auth' ('ID' INTEGER, 'MACAddress' TEXT, 'DateCreated' TEXT, 'ClientID' TEXT);");
                    }

                    int table_Calendar = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Calendar';");
                    if (table_Calendar.Equals(0)) {
                        await connection.ExecuteAsync("CREATE TABLE 'Calendar' ('ID' INTEGER, 'Name' TEXT, 'Start' TEXT, 'Finish' TEXT);");
                    }

                    int table_Events = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' and name='Events';");
                    if (table_Events.Equals(0)) {
                        await connection.ExecuteAsync("CREATE TABLE 'Events' ('Runtime' TEXT, 'Date' TEXT)");
                    }
                }

                catch (Exception ex)
                {
                    classLogger.Error(ex.Message, ex);

                    return Task.FromException(ex);
                }

                return Task.CompletedTask;
            }
        }

        public async Task<ConfigurationModel> GetConfiguration()
        {
            ConfigurationModel Response = new ConfigurationModel();
            List<CalenderModel> Results = new List<CalenderModel>();

            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    Response = Connection.QueryFirstOrDefault<ConfigurationModel>("SELECT ID, Firstname, Lastname, Start_time, Finish_time, CalenderImport FROM Configuration WHERE ID = '1'");

                    string Calender = Connection.QueryFirstOrDefault<string>("SELECT Calender FROM Configuration WHERE ID = '1'");

                    Results = JsonConvert.DeserializeObject<List<CalenderModel>>(Calender);
                    Response.calender = Results.ToArray();

                    string days = Connection.QueryFirstOrDefault<string>("SELECT workingdays FROM Configuration WHERE ID = '1'"); ;
                    Response.workingDays = JsonConvert.DeserializeObject<string[]>(days);

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }

            return Response;
        }

        public async Task<int> ConfigurationExists()
        {
            int configExists = 0;

            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    configExists = Connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Configuration WHERE ID = 1;");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }

            return configExists;
        }

        public async Task InsertConfiguration(ConfigurationModel Configuration)
        {
            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    // CREATE A METHOD THAT GETS THE CONFIGURATION - FINDS ITS ID THEN INCREMENTS IT BY +1 for the next time [AND CAN BE USED FOR MORE THAN ONE TYPE]
                    Connection.Execute("INSERT INTO Configuration " +
                        "(ID, Firstname, Lastname, WorkingDays, Start_time, Finish_time, CalenderImport, Calender) VALUES "
                        + "(1,'" + Configuration.firstname
                        + "', '" + Configuration.lastname
                        + "', '" + JsonConvert.SerializeObject(Configuration.workingDays)
                        + "', '" + Configuration.StartTime.TimeOfDay
                        + "', '" + Configuration.EndTime.TimeOfDay
                        + "', '" + Configuration.calenderImport
                        + "', '" + JsonConvert.SerializeObject(Configuration.calender) + "');");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }
        }

        public async Task<PromptModel> GetPrompt(string type)
        {
            PromptModel Response = new PromptModel();

            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    Response = Connection.QuerySingle<PromptModel>("SELECT * FROM Prompts WHERE Type = '" + type + "' ORDER BY RANDOM() LIMIT 1;");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }

            return Response;
        }

        public async Task InsertPrompt(PromptModel Prompt)
        {
            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    Connection.Execute("INSERT INTO Prompts (ID, Type, Text) VALUES ('" + string.Empty + "','" + Prompt.Type + "','" + Prompt.Text + "');");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }
        }

        public async Task<UsedPromptsModel> GetUsedPrompts(int PromptID)
        {
            UsedPromptsModel Response = new UsedPromptsModel();

            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    Response = Connection.QuerySingle<UsedPromptsModel>("SELECT * FROM UsedPrompts WHERE PromptID = '" + PromptID + "' ORDER BY LastUsed DESC;");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }

            return Response;
        }

        public async Task InsertUsedPrompt(UsedPromptsModel UsedPrompt)
        {
            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    Connection.Execute("INSERT INTO UsedPrompts (ID, PromptID, LastUsed) VALUES ('" + string.Empty + "', '" + UsedPrompt.PromptID + "', '" + UsedPrompt.LastUsed + "');");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }
        }

        public async Task InsertAuth(AuthorizeModel Authentication)
        {
            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    Connection.Execute("INSERT INTO 'Auth' (ID, MACAddress, DateCreated, ClientID) VALUES ('" + string.Empty + "', '" + Authentication.MACAddress + "', '" + DateTime.Now + "', '" + Authentication.ClientID + "');");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }
        }

        public async Task<int> GetAuth(string MACAddress)
        {
            int Exists = 0;

            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    Exists = Connection.ExecuteScalar<int>("SELECT count(*) FROM Auth WHERE MACAddress = '" + MACAddress + "';");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }

            return Exists;
        }

        public async Task<int> UpdateAuth(string MACAddress)
        {
            int RowsAffected;

            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                RowsAffected = Connection.Execute($"UPDATE Auth SET DateCreated = '{DateTime.Now}' WHERE MACAddress = '{MACAddress}';");

                await Connection.CloseAsync();
            }

            return RowsAffected;
        }

        public async Task<DateTime[]> GetShift()
        {
            DateTime[] times = new DateTime[] { DateTime.Now };

            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    string StartShift = Connection.QuerySingle<string>("SELECT Start_time, Finish_time FROM Configuration");
                    string FinishShift = Connection.QuerySingle<string>("SELECT Finish_time FROM Configuration");

                    await Connection.CloseAsync();

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
                classLogger.Error(ex.Message, ex);
            }

            return times;
        }

        public async Task DeleteExpiredTokens()
        {
            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    await Connection.ExecuteAsync("DELETE * FROM Auth WHERE datetime('now', '-1 day') >= Generated;");

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }
        }

        public async Task InsertCalenderEvents(CalenderModel[] calendarEvents)
        {
            List<CalenderModel> Events = new List<CalenderModel>();

            try
            {
                Events = calendarEvents.ToList();

                using (SQLiteConnection Connection = await CreateConnection())
                {
                    await Connection.OpenAsync();

                    foreach (var item in calendarEvents)
                    {
                        Connection.Execute("INSERT INTO 'Calendar' (ID, Name, Start, Finish) VALUES ('" + string.Empty + "', '" + item.Name + "', '" + item.StartDate + "', '" + item.EndDate + "');");
                    }

                    await Connection.CloseAsync();
                }
            }

            catch (Exception ex)
            {
                classLogger.Error(ex.Message, ex);
            }
        }

        public async Task InsertDay(List<CalendarEvents> Events)
        {
            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    foreach (CalendarEvents E in Events)
                    {
                        await Connection.OpenAsync();

                        Connection.Execute("INSERT INTO Events (Runtime, Date) VALUES ('" + E.Runtime + "', '" + E.Event + "');");

                        await Connection.CloseAsync();
                    }
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex.Message + ex);
            }
        }

        public async Task GetDays()
        {
            IEnumerable<List<CalendarEvents>> Events;

            try
            {
                using (SQLiteConnection Connection = await CreateConnection())
                {
                    Events = Connection.Query<List<CalendarEvents>>("SELECT * FROM Events");
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex.Message + ex);
            }
        }

        public void Dispose() => GC.Collect();
    }
}