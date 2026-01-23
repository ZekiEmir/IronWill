using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace IronWill.Models.ViewModels
{
    public class CreateBookViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Author { get; set; } = string.Empty;
        
        [Display(Name = "Total Pages")]
        public int TotalPages { get; set; }
        
        [Required]
        [Display(Name = "PDF File")]
        public IFormFile PdfFile { get; set; }
    }
}
