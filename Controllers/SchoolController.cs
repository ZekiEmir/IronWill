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
                var topic = new Topic { SubjectId = subjectId, Title = title, IsCompleted = false };
                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // AJAX Action for School/Index Checkboxes
        [HttpPost]
        public async Task<IActionResult> ToggleTopicAjax(int id)
        {
            var topic = await _context.Topics.Include(t => t.Subject).ThenInclude(s => s.Topics).FirstOrDefaultAsync(t => t.Id == id);
            if (topic != null)
            {
                topic.IsCompleted = !topic.IsCompleted;
                await _context.SaveChangesAsync();
                
                // Recalculate progress
                var total = topic.Subject.Topics.Count;
                var completed = topic.Subject.Topics.Count(t => t.IsCompleted);
                var progress = total > 0 ? (int)((double)completed / total * 100) : 0;
                
                return Json(new { success = true, progress = progress });
            }
            return Json(new { success = false });
        }
    }
}
