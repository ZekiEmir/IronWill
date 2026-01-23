using System;

namespace IronWill.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Status { get; set; } = "To Read"; // "To Read", "Reading", "Finished"
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string PdfPath { get; set; } = string.Empty; // Dosya Yolu
        public string? TacticalNotes { get; set; } // Aldığın dersler/notlar
        public int Rating { get; set; } // 1-5 Yıldız
    }
}
