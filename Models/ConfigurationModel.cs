using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServiceStack;

namespace Stressless_Service.Models
{
    public class Configuration_Model
    {
        [Column("ConfigurationID")]
        public Guid ID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] WorkingDays { get; set; }
        public TimeOnly DayStartTime { get; set; }
        public TimeOnly DayEndTime { get; set; }
        public string CalenderImport { get; set; }

        [Column(TypeName = "TEXT")]
        public string Calender { get; set; }

        // Static constructor to initilize '_Model'
        public static Configuration_Model _Model;
        static Configuration_Model()
        {
            _Model = new Configuration_Model();
        }
    }

    public class ConfigurationClass
    {
        /// <summary> [Use this model...]
        /// * Transfering data from frontend
        /// * 
        /// </summary>

        public string FirstName
        {
            get => Configuration_Model._Model.FirstName;
            set => Configuration_Model._Model.FirstName = value;
        }
        public string LastName
        {
            get => Configuration_Model._Model.LastName;
            set => Configuration_Model._Model.LastName = value;
        }
        public string[] WorkingDays
        {
            get => Configuration_Model._Model.WorkingDays;
            set => Configuration_Model._Model.WorkingDays = value;
        }
        public TimeOnly DayStartTime
        {
            get => Configuration_Model._Model.DayStartTime;
            set => Configuration_Model._Model.DayStartTime = value;
        }
        public TimeOnly DayEndTime
        {
            get => Configuration_Model._Model.DayEndTime;
            set => Configuration_Model._Model.DayEndTime = value;
        }
        public string CalenderImport
        {
            get => Configuration_Model._Model.CalenderImport;
            set => Configuration_Model._Model.CalenderImport = value;
        }
        public CalenderModel[] Calender
        {
            get
            {
                List<CalenderModel> list = new();
                list = JsonConvert.DeserializeObject<List<CalenderModel>>(Configuration_Model._Model.Calender);

                return list.ToArray();
            }
            set => Configuration_Model._Model.Calender = JsonConvert.SerializeObject(value);
        }
    }
}