using Microsoft.AspNetCore.Mvc;
using IronWill.Data;
using IronWill.Models;
using System.Linq;

namespace IronWill.Controllers
{
    public class JournalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.RankService _rankService;

        public JournalController(ApplicationDbContext context, Services.RankService rankService)
        {
            _context = context;
            _rankService = rankService;
        }

        public IActionResult Index()
        {
            var logs = _context.JournalEntries.OrderByDescending(j => j.Date).ToList();
            return View(logs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JournalEntry entry)
        {
            if (ModelState.IsValid)
            {
                entry.Date = DateTime.Now;
                _context.JournalEntries.Add(entry);
                await _context.SaveChangesAsync();
                
                // XP REWARD for Journal
                await _rankService.AddXPAsync("Journal", "Nöbet Tutuldu", 50);

                return RedirectToAction(nameof(Index));
            }
            return View(entry);
        }
        
        public IActionResult Details(int id)
        {
            var entry = _context.JournalEntries.Find(id);
            if (entry == null) return NotFound();
            return View(entry);
        }

        public IActionResult Edit(int id)
        {
            var entry = _context.JournalEntries.Find(id);
            if (entry == null) return NotFound();
            return View(entry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, JournalEntry entry)
        {
            if (id != entry.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Retrieve existing validation to keep Date original or specifically update it?
                // Letting user edit content but keeping original date usually better, 
                // but user said "düzenleyebilme", assuming content mainly.
                // However, EF Core update will handle properties.
                // We should ensure the Date isn't lost if not in form, 
                // but typically View return input hidden for Id. 
                // Let's attach and modify.
                
                try
                {
                    _context.Update(entry);
                    _context.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
                {
                   if (!_context.JournalEntries.Any(e => e.Id == id)) return NotFound();
                   else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(entry);
        }
    }
}
