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

            var viewModel = new GymViewModel
            {
                History = await _context.WorkoutLogs.OrderByDescending(w => w.Date).Take(50).ToListAsync(),
                NewLog = new WorkoutLog { Date = DateTime.Now },
                TodayFoodLogs = todayFoodLogs,
                TotalProteinToday = todayFoodLogs.Sum(f => f.ProteinAmount)
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
                await _rankService.AddXPAsync("Gym", $"Antrenman: {log.ExerciseName}", 100);

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
