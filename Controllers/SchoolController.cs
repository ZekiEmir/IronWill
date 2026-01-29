using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IronWill.Data;
using IronWill.Models;

namespace IronWill.Controllers
{
    public class SchoolController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SchoolController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Topics)
                .ToListAsync();
            
            // Migration hack: If Status is null but IsCompleted is set, fix it in memory (or save)
            foreach(var s in subjects) {
                foreach(var t in s.Topics) {
                    if(string.IsNullOrEmpty(t.Status)) {
                        t.Status = t.IsCompleted ? "Done" : "Todo";
                    }
                }
            }

            return View(subjects);
        }

        public IActionResult CreateSubject()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubject(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        // Action to add a topic to a subject (Simplified for prototype)
        [HttpPost]
        public async Task<IActionResult> AddTopic(int subjectId, string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                var topic = new Topic { SubjectId = subjectId, Title = title, Status = "Todo", IsCompleted = false };
                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MoveTopic(int id, string status)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic != null)
            {
                topic.Status = status;
                topic.IsCompleted = (status == "Done");
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
