namespace IronWill.Models
{
    public class Topic
    {
        public int Id { get; set; }
        
        public int SubjectId { get; set; }
        
        public string Title { get; set; }
        
        public bool IsCompleted { get; set; }
        
        public string Status { get; set; } = "Todo"; // Kanban Status

        // Navigation Property
        public Subject Subject { get; set; }
    }
}
