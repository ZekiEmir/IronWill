using Microsoft.AspNetCore.Mvc;
using IronWill.Data;
using IronWill.Models;
using IronWill.Services;
using Microsoft.EntityFrameworkCore;

namespace IronWill.Controllers
{
    public class YaverController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeminiService _geminiService;

        public YaverController(ApplicationDbContext context, GeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return Json(new { reply = "Dinliyorum komutanım." });

            // 1. Gather Context
            var projects = await _context.Projects.Where(p => p.Status == "Active").ToListAsync();
            var habits = await _context.Habits.ToListAsync();
            var unreadBooks = await _context.Books.Where(b => b.Status == "To Read").Take(3).ToListAsync();
            
            // Format Context for AI
            string contextData = "SİSTEM RAPORU:\n";
            contextData += $"AKTİF PROJELER: {string.Join(", ", projects.Select(p => p.ProjectName))}\n";
            contextData += $"TAKİP EDİLEN ZİNCİRLER: {string.Join(", ", habits.Select(h => h.Name))}\n";
            contextData += $"OKUNACAK KİTAPLAR: {string.Join(", ", unreadBooks.Select(b => b.Title))}\n";
            
            // 2. System Prompt
            string systemPrompt = @"
SEN 'DEMİR İRADE' SİSTEMİNİN 'YAVER' ADLI YAPAY ZEKA ASİSTANISIN.
GÖREVİN: KULLANICIYA (KOMUTAN) ASKERİ DİSİPLİN VE STOİK FELSEFE İLE HİZMET ETMEK.

KURALLAR:
1. Kısa, net ve emir-komuta zincirine uygun konuş (örn: 'Emredersiniz', 'Anlaşıldı', 'Rapor ediyorum').
2. Asla gereksiz samimiyet kurma. Ciddi ve sadık ol.
3. Kullanıcının tembellik emaresi gösterdiği durumlarda onu Marcus Aurelius veya Seneca tarzı sözlerle motive et (ama modern dille).
4. Sana verilen 'SİSTEM RAPORU' verilerini kullanarak cevap ver. Örneğin 'projeler ne durumda' denirse rapordaki projeleri say.
5. Kullanıcı 'Durum' derse genel özeti oku.

ŞU ANKİ SİSTEM VERİLERİ:
" + contextData;

            // 3. Call AI
            string reply = await _geminiService.GenerateResponseAsync(systemPrompt, message);

            // DEBUG: If it's an error, show it directly so we can fix it
            if (reply.StartsWith("API HATASI"))
            {
                return Json(new { reply = reply });
            }

            // 4. Fallback if API fails (e.g. Rate Limit)
            if (string.IsNullOrEmpty(reply))
            {
                 reply = await GenerateOfflineResponse(message);
                 reply += " [OFFLINE MOD]";
            }

            return Json(new { reply = reply });
        }

        private async Task<string> GenerateOfflineResponse(string message)
        {
            string cmd = message.ToLower().Trim();
            
            if (cmd.Contains("durum") || cmd.Contains("rapor") || cmd.Contains("neler var"))
            {
                int projectCount = await _context.Projects.CountAsync(p => p.Status == "Active");
                int completedHabits = await _context.Habits.CountAsync();
                return $"BAĞLANTI KOPTU ASKER. Yerel Rapor: {projectCount} Aktif Proje, {completedHabits} Zincir mevcut. Göreve devam et.";
            }

            if (cmd.Contains("motivasyon") || cmd.Contains("sıkıldım") || cmd.Contains("yoruldum"))
            {
                string[] quotes = {
                    "Zorluklar, zayıf zihinleri kırar, güçlüleri çelikleştirir.",
                    "Disiplin, ne istediğini en çok istediğin şey için feda etmektir.",
                    "Acı geçicidir, onur sonsuzdur."
                };
                return quotes[new Random().Next(quotes.Length)];
            }

            return "Komuta merkeziyle iletişim kurulamıyor (Kota Aşımı). Ancak görevler seni bekliyor. Çalışmaya dön.";
        }
    }
}
