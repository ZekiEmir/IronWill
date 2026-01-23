using System.Collections.Generic;

namespace IronWill.Models
{
    public class Subject
    {
        public int Id { get; set; }
        
        public string Name { get; set; } // e.g., "Engineering Math"
        
        public int TargetHoursPerWeek { get; set; }
        
        public List<Topic> Topics { get; set; } = new List<Topic>();
    }
}
