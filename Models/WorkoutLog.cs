namespace IronWill.Models
{
    public class WorkoutLog
    {
        public int Id { get; set; }
        
        public string MuscleGroup { get; set; } // "Chest", "Back"
        
        public string ExerciseName { get; set; }
        
        public double WeightKg { get; set; }
        
        public int Reps { get; set; }
        
        public DateTime Date { get; set; }
    }
}
