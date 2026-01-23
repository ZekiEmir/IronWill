using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IronWill.Data;
using IronWill.Models;

namespace IronWill.Controllers
{
    public class HabitController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.RankService _rankService;

        public HabitController(ApplicationDbContext context, Services.RankService rankService)
        {
            _context = context;
            _rankService = rankService;
        }

        // ... (Index, Create omitted for brevity, adding XP logic to Relapse)

        // GET: Habit
        public async Task<IActionResult> Index()
        {
            return View(await _context.Habits.ToListAsync());
        }

        // GET: Habit/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Habit/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Icon")] Habit habit)
        {
            if (ModelState.IsValid)
            {
                habit.LastRelapseDate = DateTime.Now; // Start fresh
                habit.BestStreakDays = 0;
                _context.Add(habit);
                await _context.SaveChangesAsync();
                
                // New Habit XP
                await _rankService.AddXPAsync("Habit", $"Yeni Hedef: {habit.Name}", 20);

                return RedirectToAction(nameof(Index));
            }
            return View(habit);
        }

        // GET: Habit/Relapse/5
        public async Task<IActionResult> Relapse(int? id)
        {
            if (id == null) return NotFound();

            var habit = await _context.Habits.FindAsync(id);
            if (habit == null) return NotFound();

            return View(habit);
        }

        // POST: Habit/Relapse/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Relapse(int id, string triggerReason, string notes)
        {
            var habit = await _context.Habits.FindAsync(id);
            if (habit == null) return NotFound();

            // Log Relapse
            var log = new RelapseLog
            {
                HabitId = habit.Id,
                Date = DateTime.Now,
                TriggerReason = triggerReason,
                Notes = notes
            };
            _context.RelapseLogs.Add(log);

            // XP PENALTY
            await _rankService.AddXPAsync("Relapse", $"İrade Bozgunu: {habit.Name}", -250);

            // Reset Habit
            // Check if current streak was best
            int currentStreak = (DateTime.Now - habit.LastRelapseDate).Days;
            if (currentStreak > habit.BestStreakDays)
            {
                habit.BestStreakDays = currentStreak;
            }
            habit.LastRelapseDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Habit/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var habit = await _context.Habits.FindAsync(id);
            if (habit == null) return NotFound();

            return View(habit);
        }

        // POST: Habit/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Icon,LastRelapseDate,BestStreakDays")] Habit habit)
        {
            if (id != habit.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(habit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                   if (!_context.Habits.Any(e => e.Id == id)) return NotFound();
                   else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(habit);
        }
    }
}
