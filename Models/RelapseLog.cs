using System.ComponentModel.DataAnnotations;

namespace IronWill.Models
{
    public class RelapseLog
    {
        public int Id { get; set; }
        
        public int HabitId { get; set; }
        
        public DateTime Date { get; set; }
        
        public string TriggerReason { get; set; } // e.g., "Stress", "Boredom"
        
        public string Notes { get; set; }
        
        // Navigation Property (Optional but good for EF)
        public Habit Habit { get; set; }
    }
}
