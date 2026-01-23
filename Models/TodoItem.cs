using System;

namespace IronWill.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
