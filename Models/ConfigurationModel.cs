namespace Stressless_Service.Models
{
    public class ConfigurationModel
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string WorkingDays { get; set; }
        public string Start_time { get; set; }
        public string Finish_time { get; set; }
        public string CalenderImport { get; set; }
        public CalenderModel Calender { get; set; }
    }

    public class CalenderModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Start_time { get; set; }
        public string End_time { get; set; }
    }
}
