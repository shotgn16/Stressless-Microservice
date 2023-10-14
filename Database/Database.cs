using System.Data.SQLite;
using System.Net;
using System.Reflection;
using System.Text.Json.Nodes;
using Dapper;
using Microsoft.AspNetCore.Mvc;
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
                        connection.Execute("CREATE TABLE 'Configuration' ('ID' INTEGER, 'Firstname' TEXT, 'Lastname' INTEGER, 'WorkingDays' TEXT, 'Start_time' TEXT, 'Finish_time' TEXT, 'CalenderImport' TEXT, 'Calender' TEXT, PRIMARY KEY('ID'));");
                    }

                    int table_Prompts = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Prompts';");
                    if (table_Configuration.Equals(0)) {
                        connection.Execute("CREATE TABLE 'Prompts' ('ID' INTEGER, 'Type' TEXT, 'Text' TEXT, PRIMARY KEY('ID'));");
                    }

                    int table_UsedPrompts = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='UsedPrompts';");
                    if (table_UsedPrompts.Equals(0)) {
                        connection.Execute("CREATE TABLE 'UsedPrompts' ('ID' INTEGER, 'PromptID' INTEGER, 'LastUsed' TEXT, FOREIGN KEY('PromptID') REFERENCES 'Prompts', PRIMARY KEY('ID'));");
                    }

                    int table_Auth = connection.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Auth';");
                    if (table_Auth.Equals(0)) {
                        connection.Execute("CREATE TABLE 'Auth' ('ID' INTEGER, 'ClientMAC' TEXT, 'Generated' TEXT, 'AudienceCode' TEXT);");
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
            List<CalenderModel> Results = new List<CalenderModel>();

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

            return Response;
        }

        public async Task InsertConfiguration(ConfigurationModel Configuration)
        {
            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();
                 
                Connection.Execute("INSERT INTO Configuration (ID, Firstname, Lastname, WorkingDays, Start_time, Finish_time, CalenderImport, Calender) VALUES ('" + Configuration.id + "', '" + Configuration.firstname + "', '" + Configuration.lastname + "', '" + JsonConvert.SerializeObject(Configuration.workingDays) + "', '" + Configuration.day_Start + "', '" + Configuration.day_End + "', '" + Configuration.calenderImport + "', '" + JsonConvert.SerializeObject(Configuration.calender) + "');");

                await Connection.CloseAsync();
            }
        }

        public async Task<PromptModel> GetPrompt(string type)
        {
            PromptModel Response;

            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                Response = Connection.QuerySingle<PromptModel>("SELECT * FROM Prompts WHERE Type = '" + type + "' ORDER BY RANDOM() LIMIT 1;");

                await Connection.CloseAsync();
            }

            return Response;
        }

        public async Task InsertPrompt(PromptModel Prompt)
        {
            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                Connection.Execute("INSERT INTO Prompts (ID, Type, Text) VALUES ('" + string.Empty + "','" + Prompt.Type + "','" + Prompt.Text + "');");

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

                Connection.Execute("INSERT INTO UsedPrompts (ID, PromptID, LastUsed) VALUES ('" + string.Empty + "', '" + UsedPrompt.PromptID + "', '" + UsedPrompt.LastUsed + "');");

                await Connection.CloseAsync();
            }
        }

        public async Task InsertAuth(AuthorizeModel Authentication)
        {
            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                Connection.Execute("INSERT INTO 'Auth' (ID, ClientMAC, Generated, AudienceCode) VALUES ('" + string.Empty + "', '" + Authentication.IpAddress + "', '" + DateTime.Now + "', '" + Authentication.AudienceCode + "');");

                await Connection.CloseAsync();
            }
        }

        public async Task<int> GetAuth(string IPAddress)
        {
            int Exists;

            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                Exists = Connection.ExecuteScalar<int>("SELECT count(*) FROM Auth WHERE ClientMAC = '" + IPAddress + "';");

                await Connection.CloseAsync();
            }

            return Exists;
        }

        public async Task<DateTime[]> GetShift()
        {
            DateTime[] times;

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

            return times;
        }

        public async Task DeleteExpiredTokens()
        {
            using (SQLiteConnection Connection = await CreateConnection())
            {
                await Connection.OpenAsync();

                await Connection.ExecuteAsync("DELETE * FROM Auth WHERE datetime('now', '-1 day') >= Generated;");

                await Connection.CloseAsync();
            }
        }

        public void Dispose() => GC.Collect();
    }
}