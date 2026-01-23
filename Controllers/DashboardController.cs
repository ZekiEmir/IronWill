using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IronWill.Data;
using IronWill.Models;
using IronWill.Models.ViewModels;

namespace IronWill.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.RankService _rankService;

        public DashboardController(ApplicationDbContext context, Services.RankService rankService)
        {
            _context = context;
            _rankService = rankService;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch Habits
            var habits = await _context.Habits.ToListAsync();
            
            // Calculate Total Clean Days (Sum of clean days for all habits? Or max streak? "The Chain" usually implies individual chains. 
            // Spec says "Discipline Score" based on study hours and clean streak days.
            // Let's sum up current streaks for now or average them.
            // "Calculate 'Days Clean' dynamically: (DateTime.Now - LastRelapseDate).Days."
            
            int totalCleanDays = 0;
            foreach(var h in habits)
            {
                 totalCleanDays += (DateTime.Now - h.LastRelapseDate).Days;
            }

            // Fetch Projects
            var activeProjects = await _context.Projects
                .Where(p => p.Status == "Active")
                .ToListAsync();
            
            var allProjects = await _context.Projects.ToListAsync();
            decimal totalEarnings = allProjects.Sum(p => p.PaidAmount);

            // Fetch Study Hours (This Week)
             var startOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
             var studySessions = await _context.StudySessions
                 .Where(s => s.Date >= startOfWeek)
                 .ToListAsync();
             int weeklyStudyMinutes = studySessions.Sum(s => s.DurationMinutes);
             int weeklyStudyHours = weeklyStudyMinutes / 60;

            // Simple Discipline Score Algorithm: (CleanDays * 10) + (StudyHours * 5)
            int disciplineScore = (totalCleanDays * 10) + (weeklyStudyHours * 5);
            if (disciplineScore > 100) disciplineScore = 100;

            var lastJournal = _context.JournalEntries.OrderByDescending(j => j.Date).FirstOrDefault();
            
            // Fetch Todos
            var todos = await _context.TodoItems.OrderByDescending(t => t.CreatedAt).ToListAsync();

            // Rank System Calculation
            int totalXP = await _rankService.GetTotalXPAsync();
            var rankInfo = _rankService.GetRank(totalXP);

            var viewModel = new DashboardViewModel
            {
                DisciplineScore = disciplineScore,
                TotalCleanDays = totalCleanDays,
                ActiveProjectsCount = activeProjects.Count,
                TotalProjectEarnings = totalEarnings,
                WeeklyStudyHours = weeklyStudyHours,
                Habits = habits,
                ActiveProjects = activeProjects,
                Todos = todos,
                DailyQuote = "Sadece hayal gücümüzde, gerçekte olduğundan daha fazla acı çekeriz. - Seneca",
                LastJournalDate = lastJournal?.Date,
                
                // Rank Data
                CurrentXP = totalXP,
                RankTitle = rankInfo.Title,
                RankIcon = rankInfo.Icon,
                RankColor = rankInfo.Color,
                NextRankXP = rankInfo.NextRankXP,
                LevelProgress = rankInfo.ProgressPercent
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CheckMorningStatus()
        {
            var today = DateTime.Today;
            var logExists = _context.MorningLogs.Any(m => m.Date.Date == today);
            return Json(new { isCompleted = logExists });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitMorningLog([FromBody] MorningLog log)
        {
            if (log == null) return BadRequest("Invalid Data");

            log.Date = DateTime.Now;
            _context.MorningLogs.Add(log);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddTodo(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                _context.TodoItems.Add(new TodoItem { Text = text, IsCompleted = false });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleTodo(int id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item != null)
            {
                item.IsCompleted = !item.IsCompleted;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item != null)
            {
                _context.TodoItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
