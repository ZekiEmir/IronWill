using System;

namespace IronWill.Models
{
    public class XPHistory
    {
        public int Id { get; set; }
        public string Source { get; set; } // e.g. "Gym", "Journal", "Relapse"
        public string Description { get; set; } // e.g. "Leg Day Workout", "Morning Log"
        public int Points { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
