using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IronWill.Data;
using IronWill.Models;
using IronWill.Models.ViewModels;

namespace IronWill.Controllers
{
    public class GymController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.RankService _rankService;

        public GymController(ApplicationDbContext context, Services.RankService rankService)
        {
            _context = context;
            _rankService = rankService;
        }

        public async Task<IActionResult> Index()
        {
            var startOfDay = DateTime.Today;
            var todayFoodLogs = await _context.FoodLogs.Where(f => f.Date >= startOfDay).ToListAsync();
            
            // Chart Logic: Last 7 Days Volume
            var last7Days = DateTime.Today.AddDays(-6);
            var chartLogs = await _context.WorkoutLogs
                .Where(w => w.Date >= last7Days)
                .ToListAsync();

            var labels = new List<string>();
            var values = new List<double>();

            for (int i = 0; i < 7; i++)
            {
                var day = last7Days.AddDays(i);
                labels.Add(day.ToString("dd MMM")); // 29 Jan
                
                var daysLogs = chartLogs.Where(l => l.Date.Date == day.Date).ToList();
                double dailyVolume = daysLogs.Sum(l => l.WeightKg * l.Reps);
                values.Add(dailyVolume);
            }

            var viewModel = new GymViewModel
            {
                History = await _context.WorkoutLogs.OrderByDescending(w => w.Date).Take(20).ToListAsync(),
                NewLog = new WorkoutLog { Date = DateTime.Now },
                TodayFoodLogs = todayFoodLogs,
                TotalProteinToday = todayFoodLogs.Sum(f => f.ProteinAmount),
                ChartLabels = labels,
                ChartValues = values
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Log(GymViewModel viewModel)
        {
            // Workout Log Logic
            if (viewModel.NewLog != null && !string.IsNullOrEmpty(viewModel.NewLog.ExerciseName)) 
            {
                var log = viewModel.NewLog;
                log.Date = DateTime.Now;
                _context.Add(log);
                await _context.SaveChangesAsync();

                // XP REWARD
                await _rankService.AddXPAsync("Gym", $"Antrenman: {log.ExerciseName}", 20);

                return RedirectToAction(nameof(Index));
            }
            
            // Reload history if failed
            viewModel.History = await _context.WorkoutLogs.OrderByDescending(w => w.Date).Take(50).ToListAsync();
            // Reload Nutrition
            var startOfDay = DateTime.Today;
            var todayFoodLogs = await _context.FoodLogs.Where(f => f.Date >= startOfDay).ToListAsync();
            viewModel.TodayFoodLogs = todayFoodLogs;
            viewModel.TotalProteinToday = todayFoodLogs.Sum(f => f.ProteinAmount);

            return View("Index", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddFoodLog(string foodName, int protein)
        {
            if (!string.IsNullOrEmpty(foodName) && protein > 0)
            {
                var log = new FoodLog
                {
                    FoodName = foodName,
                    ProteinAmount = protein,
                    Date = DateTime.Now
                };
                _context.Add(log);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
