using System;

namespace IronWill.Models
{
    public class MorningLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool BedMade { get; set; }
        public bool WaterDrank { get; set; }
        public string DailyMainGoal { get; set; } = string.Empty;
    }
}
