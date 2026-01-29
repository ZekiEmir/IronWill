using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IronWill.Data;
using IronWill.Models;
using IronWill.Models.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace IronWill.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly Services.RankService _rankService;

        public BooksController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, Services.RankService rankService)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _rankService = rankService;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.PdfFile != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "books");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PdfFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PdfFile.CopyToAsync(fileStream);
                    }
                }

                var book = new Book
                {
                    Title = model.Title,
                    Author = model.Author,
                    TotalPages = model.TotalPages,
                    Status = "To Read",
                    PdfPath = "/uploads/books/" + uniqueFileName,
                    TacticalNotes = "",
                    Rating = 0,
                    CurrentPage = 0
                };

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Books/Read/5
        public async Task<IActionResult> Read(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            // Update status to Reading if it was To Read
            if (book.Status == "To Read")
            {
                book.Status = "Reading";
                _context.Update(book);
                await _context.SaveChangesAsync();
            }

            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> SaveNotes(int id, string notes)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Check if note is significant to reward XP (simple check)
            if (string.IsNullOrEmpty(book.TacticalNotes) && !string.IsNullOrEmpty(notes))
            {
                 await _rankService.AddXPAsync("Intel", "Taktiksel Kitap Notu", 60);
            }

            book.TacticalNotes = notes;
            _context.Update(book);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateProgress(int id, int currentPage)
        {
             var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.CurrentPage = currentPage;
            
            // XP REWARD for Reading Session
            await _rankService.AddXPAsync("Books", $"Kitap Okuma: {book.Title}", 15);

            if(book.CurrentPage >= book.TotalPages && book.TotalPages > 0 && book.Status != "Finished")
            {
                book.Status = "Finished";
                await _rankService.AddXPAsync("Books", $"Kitap Bitirildi: {book.Title}", 200);
            }
            
            _context.Update(book);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
