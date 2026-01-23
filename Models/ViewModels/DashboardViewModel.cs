using IronWill.Models;
using System.Collections.Generic;

namespace IronWill.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int DisciplineScore { get; set; }
        public int TotalCleanDays { get; set; }
        public int ActiveProjectsCount { get; set; }
        public decimal TotalProjectEarnings { get; set; }
        public decimal TargetEarningsGoal { get; set; } = 50000; // Example goal
        public int WeeklyStudyHours { get; set; }
        
        public List<Habit> Habits { get; set; }
        public List<Project> ActiveProjects { get; set; }
        public List<TodoItem> Todos { get; set; } = new List<TodoItem>();
        
        // Stoic Quote
        public string DailyQuote { get; set; }
        public DateTime? LastJournalDate { get; set; }

        // Rank System
        public string RankTitle { get; set; }
        public string RankIcon { get; set; }
        public string RankColor { get; set; }
        public int CurrentXP { get; set; }
        public int NextRankXP { get; set; }
        public int LevelProgress { get; set; }
    }
}
