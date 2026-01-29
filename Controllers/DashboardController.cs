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

            // Heatmap Calculations (Last 90 Days)
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-90);
            
            var logs = await _context.MorningLogs
                .Where(m => m.Date >= startDate && m.Date <= endDate.AddDays(1)) // Include today even if it's late
                .ToListAsync();

            var heatmapData = new List<HeatmapItem>();
            for (var d = startDate; d <= endDate; d = d.AddDays(1))
            {
                var log = logs.FirstOrDefault(l => l.Date.Date == d);
                int level = 0;
                int count = 0;

                if (log != null)
                {
                    int trueCount = 0;
                    if (log.BedMade) trueCount++;
                    if (log.WaterDrank) trueCount++;
                    if (!string.IsNullOrEmpty(log.DailyMainGoal)) trueCount++;
                    
                    count = trueCount;
                    if (trueCount == 3) level = 2; // Full Green
                    else if (trueCount > 0) level = 1; // Pale Green
                }

                heatmapData.Add(new HeatmapItem
                {
                    Date = d,
                    Level = level,
                    Count = count
                });
            }

            // Step 4: Visual Intelligence (Charts - Last 7 Days)
            var chartStartDate = DateTime.Today.AddDays(-6);
            
            // 1. Discipline (XP) Chart
            var xpLogs = await _context.XPHistory.Where(x => x.Date >= chartStartDate).ToListAsync();
            
            // 2. Sleep Chart
            var sleepLogs = await _context.JournalEntries.Where(j => j.Date >= chartStartDate).ToListAsync();

            var chartLabels = new List<string>();
            var disciplineValues = new List<int>();
            var sleepValues = new List<double>();

            for (int i = 0; i < 7; i++)
            {
                var day = chartStartDate.AddDays(i);
                chartLabels.Add(day.ToString("dd MMM"));

                // Sum XP for that day
                int dayXP = xpLogs.Where(x => x.Date.Date == day.Date).Sum(x => x.Points);
                disciplineValues.Add(dayXP);

                // Get Sleep for that day (last entry if multiple, or avg)
                var sleepEntry = sleepLogs.Where(j => j.Date.Date == day.Date).OrderByDescending(j => j.Date).FirstOrDefault();
                sleepValues.Add(sleepEntry != null ? sleepEntry.SleepHours : 0);
            }

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
                LevelProgress = rankInfo.ProgressPercent,
                HeatmapData = heatmapData,
                
                // Step 2 Data
                NextMission = todos.FirstOrDefault(t => !t.IsCompleted),
                PendingEarnings = activeProjects.Sum(p => p.PaidAmount),
                
                // Step 4 Data
                ChartLabels = chartLabels,
                DisciplineChartValues = disciplineValues,
                SleepChartValues = sleepValues
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

            // XP REWARD
            await _rankService.AddXPAsync("Morning", "Sabah İçtiması", 10);

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

                // XP REWARD if completed
                if (item.IsCompleted)
                {
                    await _rankService.AddXPAsync("Mission", "Görev Tamamlandı", 50);
                }
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

        [HttpPost]
        public async Task<IActionResult> SubmitNightLog(NightLogViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entry = new JournalEntry
                {
                    Date = DateTime.Now,
                    Title = $"GECE RAPORU - {DateTime.Now:dd.MM.yyyy}",
                    Mood = model.WillControlled ? "Disiplinli" : "Zayıf",
                    SelfRating = model.WillControlled ? 10 : 5,
                    SleepHours = model.SleepHours,
                    Content = $@"
**İrade Hakimiyeti:** {(model.WillControlled ? "Evet" : "Hayır")}

**Zaman Kaybı:**
{model.TimeWasted}

**Yarının Hedefi:**
{model.TomorrowGoal}
"
                };

                _context.JournalEntries.Add(entry);
                await _context.SaveChangesAsync();

                // XP Reward
                await _rankService.AddXPAsync("NightWatch", "Gece Raporu", 30);

                TempData["NightLogSuccess"] = "Rapor alındı. İyi istirahatler Asker. Görev tamamlandı.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
