using Microsoft.EntityFrameworkCore;
using IronWill.Models;

namespace IronWill.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Habit> Habits { get; set; }
        public DbSet<RelapseLog> RelapseLogs { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<StudySession> StudySessions { get; set; }
        public DbSet<WorkoutLog> WorkoutLogs { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<MorningLog> MorningLogs { get; set; }
        public DbSet<FoodLog> FoodLogs { get; set; }
        public DbSet<SocialLog> SocialLogs { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<XPHistory> XPHistory { get; set; }
    }
}
