namespace Stressless_Service.Models
{
    public class CalenderModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly EventDate { get; set; }
    }
}
