using IronWill.Data;
using IronWill.Models;
using Microsoft.EntityFrameworkCore;

namespace IronWill.Services
{
    public class RankService
    {
        private readonly ApplicationDbContext _context;

        public RankService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddXPAsync(string source, string description, int points)
        {
            var log = new XPHistory
            {
                Source = source,
                Description = description,
                Points = points,
                Date = DateTime.Now
            };
            _context.XPHistory.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalXPAsync()
        {
            return await _context.XPHistory.SumAsync(x => x.Points);
        }

        public (string Title, string Icon, string Color, int NextRankXP, int ProgressPercent) GetRank(int totalXP)
        {
            if (totalXP < 500) return ("ER", "fas fa-chess-pawn", "text-muted", 500, (totalXP * 100) / 500);
            if (totalXP < 1200) return ("ONBAŞI", "fas fa-angle-up", "text-white", 1200, ((totalXP - 500) * 100) / 700);
            if (totalXP < 2500) return ("ÇAVUŞ", "fas fa-angle-double-up", "text-warning", 2500, ((totalXP - 1200) * 100) / 1300);
            if (totalXP < 5000) return ("TEĞMEN", "fas fa-star", "text-info", 5000, ((totalXP - 2500) * 100) / 2500);
            if (totalXP < 10000) return ("YÜZBAŞI", "fas fa-star", "text-neon", 10000, ((totalXP - 5000) * 100) / 5000);
            if (totalXP < 50000) return ("BİNBAŞI", "fas fa-crown", "text-danger-glow", 50000, ((totalXP - 10000) * 100) / 40000);
            
            return ("MAREŞAL", "fas fa-chess-king", "text-gold", 999999, 100);
        }
    }
}
