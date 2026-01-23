namespace IronWill.Models
{
    public class StudySession
    {
        public int Id { get; set; }
        
        public int SubjectId { get; set; }
        
        public int DurationMinutes { get; set; }
        
        public int EfficiencyScore { get; set; } // 1-10
        
        public DateTime Date { get; set; }
        
        // Navigation Property
        public Subject Subject { get; set; }
    }
}
