using System;

namespace IronWill.Models
{
    public class FoodLog
    {
        public int Id { get; set; }
        public string FoodName { get; set; } = string.Empty; // e.g., "Chicken Breast"
        public int ProteinAmount { get; set; } // e.g., 25g
        public DateTime Date { get; set; }
    }
}
