using System;

namespace IronWill.Models.ViewModels
{
    public class HeatmapItem
    {
        public DateTime Date { get; set; }
        public int Level { get; set; } // 0: None, 1: Partial, 2: Full
        public int Count { get; set; } // Number of completed items (for tooltip)
    }
}
