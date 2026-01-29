using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IronWill.Data;
using IronWill.Models;

namespace IronWill.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects.ToListAsync();
            
            // Financial Stats
            ViewBag.TotalContract = projects.Sum(p => p.ContractAmount);
            ViewBag.TotalReceived = projects.Sum(p => p.PaidAmount);
            ViewBag.TotalPending = projects.Sum(p => p.ContractAmount - p.PaidAmount);

            // Chart Data: Income by Client
            var incomeByClient = projects
                .GroupBy(p => p.ClientName)
                .Select(g => new { Client = g.Key, Amount = g.Sum(p => p.PaidAmount) })
                .ToList();

            ViewBag.ChartLabels = incomeByClient.Select(x => x.Client).ToList();
            ViewBag.ChartValues = incomeByClient.Select(x => x.Amount).ToList();

            return View(projects);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }
    }
}
