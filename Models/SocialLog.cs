using System;

namespace IronWill.Models
{
    public class SocialLog
    {
        public int Id { get; set; }
        public string PersonName { get; set; } = string.Empty; // e.g., "Metehan"
        public DateTime LastContactDate { get; set; }
        public string Notes { get; set; } = string.Empty; // Gift ideas, important updates
    }
}
