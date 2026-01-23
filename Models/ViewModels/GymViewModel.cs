using IronWill.Models;

namespace IronWill.Models.ViewModels
{
    public class GymViewModel
    {
        public WorkoutLog NewLog { get; set; } = new WorkoutLog();
        public List<WorkoutLog> History { get; set; } = new List<WorkoutLog>();
        
        // Nutrition
        public List<FoodLog> TodayFoodLogs { get; set; } = new List<FoodLog>();
        public int TotalProteinToday { get; set; }
    }
}
