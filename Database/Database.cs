using System.Data.SQLite;
using System.Reflection;
using System.Text.Json.Nodes;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stressless_Service.Models;

namespace Stressless_Service.Database
{
    public class database : IDisposable
    {
        public database()
        {
            dbBuild();
        }

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
                // Log
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
                // Log
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
                        connection.Execute("CREATE TABLE 'Configuration' ('ID' INTEGER, 'Firstname' TEXT, 'Lastname' INTEGER, 'WorkingDays' TEXT, 'Start_time' TEXT, 'Finish_time' TEXT, 'Calender' TEXT, PRIMARY KEY('ID'));");
                    }

                    int table_Prompts = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Prompts';");
                    if (table_Configuration.Equals(0)) {
                        connection.Execute("CREATE TABLE 'Prompts' ('ID' INTEGER, 'Type' TEXT, 'Text' TEXT, PRIMARY KEY('ID'));");
                    }

                    int table_UsedPrompts = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='UsedPrompts';");
                    if (table_UsedPrompts.Equals(0)) {
                        connection.Execute("CREATE TABLE 'UsedPrompts' ('ID' INTEGER, 'PromptID' INTEGER, 'LastUsed' TEXT, FOREIGN KEY('PromptID') REFERENCES 'Prompts', PRIMARY KEY('ID'));");
                    }
                }

                catch (Exception ex)
                {
                    // Log

                    return Task.FromException(ex);
                }

                return Task.CompletedTask;
            }
        }

        public async Task <ConfigurationModel> GetConfiguration()
        {
            ConfigurationModel Response;

            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                string Calender = Connection.QuerySingleOrDefault<string>("SELECT * FROM Configuration");

                if (Calender.StartsWith("http"))
                {
                    Response = Connection.QuerySingle<ConfigurationModel>("SELECT * FROM Configuration");
                    
                }

                else
                {

                }

                

                await Connection.CloseAsync();
            }

            return Response;
        }

        public async Task InsertConfiguration(ConfigurationModel Configuration)
        {
            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                string SQL = "";

                if (Configuration.CalenderImport != "string")
                {
                    SQL = "INSERT INTO Configuration (ID, Firstname, Lastname, WorkingDays, Start_time, Finish_time, Calender) VALUES ('" + Configuration.ID + "', '" + Configuration.Firstname + "', '" + Configuration.Lastname + "', '" + Configuration.WorkingDays + "', '" + Configuration.Start_time + "', '" + Configuration.Finish_time + "', '" + Configuration.CalenderImport + "');";
                }

                else if (Configuration.CalenderImport == "string")
                {
                    SQL = "INSERT INTO Configuration (ID, Firstname, Lastname, WorkingDays, Start_time, Finish_time, Calender) VALUES ('" + Configuration.ID + "', '" + Configuration.Firstname + "', '" + Configuration.Lastname + "', '" + Configuration.WorkingDays + "', '" + Configuration.Start_time + "', '" + Configuration.Finish_time + "', '" + JsonConvert.SerializeObject(Configuration.Calender) + "');";
                }

                Connection.Execute(SQL);
            }
        }

        public async Task<PromptModel> GetPrompt(int ID)
        {
            PromptModel Response;

            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                Response = Connection.QuerySingle<PromptModel>("SELECT * FROM Prompts WHERE ID = '" + ID + "';");

                await Connection.CloseAsync();
            }

            return Response;
        }

        public async Task InsertPrompt(PromptModel Prompt)
        {
            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                Connection.Execute("INSERT INTO Prompts (ID, Type, Text) VALUES ('" + Prompt.ID + "','" + Prompt.Type + "','" + Prompt.Text + "');");

                await Connection.CloseAsync();
            }
        }

        public async Task<UsedPromptsModel> GetUsedPrompts(int PromptID)
        {
            UsedPromptsModel Response;

            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                Response = Connection.QuerySingle<UsedPromptsModel>("SELECT * FROM UsedPrompts WHERE PromptID = '" + PromptID + "' ORDER BY LastUsed DESC;");

                await Connection.CloseAsync();
            }

            return Response;
        }

        public async Task InsertUsedPrompt(UsedPromptsModel UsedPrompt)
        {
            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                Connection.Execute("INSERT INTO UsedPrompts (ID, PromptID, LastUsed) VALUES ('" + UsedPrompt.ID + "', '" + UsedPrompt.PromptID + "', '" + UsedPrompt.LastUsed + "');");
            }
        }

        public void Dispose() => GC.Collect();
    }
}