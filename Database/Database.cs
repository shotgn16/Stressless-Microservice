using Dapper;
using Newtonsoft.Json;
using Stressless_Service.Factories;
using Stressless_Service.Models;
using System.Data.SQLite;
using System.Reflection;

namespace Stressless_Service.Database
{
    public class database
    {
        private DatabaseFactory _databaseFactory;
        private Microsoft.Extensions.Logging.ILogger _databaseLogger;

        /// <summary>
        ///  * Creates the class logger 
        ///  * Creates and assigns a new instance of the DatabaseFactory
        ///  * Runs the database setup sequence from the DatabaseFactory instance previous created
        /// </summary>
        public database()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            _databaseLogger = loggerFactory.CreateLogger<database>();

            _databaseFactory = new DatabaseFactory(_databaseLogger);
            _databaseFactory.databasebBuild($"Data Source={Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\database.db;Version=3;");
        }

        /// <summary>
        ///  * Returns the configuration model from the database in 3 queries
        ///  * Parses the Calender string returned in one query into a List<CalenderModel> then a CalenderModel[] array
        ///  * Parses the WorkingDays string value from a query into a string Array and writes it to the configuration model
        /// </summary>
        /// 
        /// <returns> ConfigurationModel instance from the database </returns>
        public async Task<ConfigurationModel> GetConfigurationAsync()
        {
            ConfigurationModel TaskModel = new();

            try
            {
                List<CalenderModel> CalenderModel = new();
                (string Calender, string WorkingDays) StringData;

                using (SQLiteConnection conn = await _databaseFactory.CreateConnection()) 
                using (SQLiteTransaction Transaction =  conn.BeginTransaction())
                {
                    await conn.OpenAsync();

                    TaskModel = await conn.QueryFirstAsync<ConfigurationModel>("SELECT ID, FirstName, LastName, DayStartTime, DayEndTime, CalenderImport FROM Configuration WHERE ID = 1");
                    
                    StringData.Calender = await conn.QueryFirstAsync<string>("SELECT Calender FROM Configuration");
                    StringData.WorkingDays = await conn.QueryFirstAsync<string>("SELECT WorkingDays FROM Configuration");

                    // Calender (string) >>> List<CalenderModel>
                    CalenderModel = JsonConvert.DeserializeObject<List<CalenderModel>>(StringData.Calender);

                    // List<CalenderModel> >>> Configuration.CalenderModel[]
                    TaskModel.Calender = CalenderModel.ToArray();

                    // WorkingDays (string) >>> Configuration.WorkingDays[]
                    TaskModel.WorkingDays = JsonConvert.DeserializeObject<string[]>(StringData.WorkingDays);

                    await conn.CloseAsync();
                    // SUBMIT_TRANSACTION_HERE
                }
            }

            catch (Exception ex)
            {
                _databaseLogger.LogError(ex.Message, ex);
            }

            return TaskModel;
        }

        public async Task<int> CheckConfigurationExists()
        {
            int ConfigurationExists = 0;

            try
            {
                using (SQLiteConnection conn = await _databaseFactory.CreateConnection())
                using (SQLiteTransaction Transaction = conn.BeginTransaction())
                {
                    await conn.OpenAsync();


                }
            }
        }
    }
}