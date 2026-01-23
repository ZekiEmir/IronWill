using System;

namespace IronWill.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Mood { get; set; } = "Stoic"; // Default
        public int SelfRating { get; set; } // 1-10
    }
}
