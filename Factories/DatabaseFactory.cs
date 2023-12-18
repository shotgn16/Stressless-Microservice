using Dapper;
using System.Data.SQLite;
using System.Reflection;

namespace Stressless_Service.Factories
{
    public interface IDBDatabaseFactory
    {
        Task<SQLiteConnection> CreateConnection(string _connectionString);
    }

    public class DatabaseFactory : IDBDatabaseFactory, IDisposable
    {
        private readonly ILogger _logger;
        private string ConnectingString;
        public DatabaseFactory(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<SQLiteConnection> CreateConnection(string _connectionString = null)
        {
            SQLiteConnection connection = null;

            try {
                if (string.IsNullOrEmpty(_connectionString)) ConnectingString = _connectionString;
                    connection = new SQLiteConnection(ConnectingString);
            }

            catch (Exception ex) {
                _logger.LogError(ex.Message, ex);
            }

            return connection;
        }

        public async Task<bool> databasebBuild(string _connectionString) // Run this FIRST
        {
            bool dbExists = false;

            //$"Data Source={Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\database.db;Version=3;";
            ConnectingString = _connectionString;

            try {
                if (File.Exists($"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\database.db")) {
                    dbExists = true;
                    goto End;
                }

                else if (!File.Exists($"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\database.db")) {
                    SQLiteConnection.CreateFile($"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\database.db");
                }
            }

            catch (Exception ex) {
                _logger.LogError(ex.Message, ex);
            }

        End:
            await databaseSetup();
            return dbExists;
        }

        public async Task<Task> databaseSetup()
        {

            using (SQLiteConnection connection = await CreateConnection(ConnectingString)) {

                await connection.OpenAsync();

                try {
                    // Configuration  - Used to store the configuration which will define the preset for working hours, days and who is using the system.
                    int table_Configuration = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Prompts';");
                    if (table_Configuration.Equals(0))
                        await connection.ExecuteAsync("CREATE TABLE 'Configuration' ('ID' INTEGER, 'FirstName' TEXT, 'LastName' INTEGER, 'WorkingDays' TEXT, 'DayStartTime' TEXT, 'DayEndTime' TEXT, 'CalenderImport' TEXT, 'Calender' TEXT, PRIMARY KEY('ID'));");

                    // Prompts - Is stored to store a list of prompts to remind users throughout the day, such as quotes and the like.
                    int table_Prompts = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Prompts';");
                    if (table_Configuration.Equals(0))
                        await connection.ExecuteAsync("CREATE TABLE 'Prompts' ('ID' INTEGER, 'Type' TEXT, 'Text' TEXT, PRIMARY KEY('ID'));");

                    // UsedPrompts - Used to store recently used prmpts (quotes) so that they are not re-used within a specified period of time
                    int table_UsedPrompts = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='UsedPrompts';");
                    if (table_UsedPrompts.Equals(0))
                        await connection.ExecuteAsync("CREATE TABLE 'UsedPrompts' ('ID' INTEGER, 'PromptID' INTEGER, 'LastUsed' TEXT, FOREIGN KEY('PromptID') REFERENCES 'Prompts', PRIMARY KEY('ID'));");

                    // Auth - Used to store a list of authentication tokens generated within the system by matching them with their ID's (MACs) and time that they were created.
                    int table_Auth = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Auth';");
                    if (table_Auth.Equals(0))
                        await connection.ExecuteAsync("CREATE TABLE 'Auth' ('ID' INTEGER, 'MACAddress' TEXT, 'DateCreated' TEXT, 'ClientID' TEXT);");


                    int table_Calendar = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Calendar';");
                    if (table_Calendar.Equals(0))
                        await connection.ExecuteAsync("CREATE TABLE 'Calendar' ('ID' INTEGER, 'Name' TEXT, 'Location' TEXT, 'StartTime' TEXT, 'EndTime' TEXT, 'EventDate' TEXT);");

                    // Events - Used to store up events, categorized by days. Will only store up to 21 days in the past of events
                    int table_Events = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' and name='Events';");
                    if (table_Events.Equals(0))
                        await connection.ExecuteAsync("CREATE TABLE 'Events' ('Runtime' TEXT, 'Date' TEXT)");

                    // Reminder - Stores the date and time of each reminder as it happens 
                    int table_Reminder = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' and name='Reminder';");
                    if (table_Reminder.Equals(0))
                        await connection.ExecuteAsync("CREATE TABLE 'Reminders' ('Date' TEXT, 'Time' TEXT)"); 
                }

                catch (Exception ex) {
                    _logger.LogError(ex.Message, ex);
                        return Task.FromException(ex);
                }

                return Task.CompletedTask;
            }
        }

        public void Dispose() => GC.Collect();
    }
}
