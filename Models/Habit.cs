using System.ComponentModel.DataAnnotations;

namespace IronWill.Models
{
    public class Habit
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } // e.g., "No Smoking", "No Gaming"
        
        public DateTime LastRelapseDate { get; set; }
        
        public int BestStreakDays { get; set; }
        
        public string Icon { get; set; } // FontAwesome Class
    }
}
