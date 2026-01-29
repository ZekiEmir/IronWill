using System.ComponentModel.DataAnnotations;

namespace IronWill.Models.ViewModels
{
    public class NightLogViewModel
    {
        [Required]
        [Display(Name = "İradene Hakim Oldun Mu?")]
        public bool WillControlled { get; set; }

        [Required]
        [Display(Name = "Zaman Kaybı Oldu Mu?")]
        public string TimeWasted { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Yarın İçin Tek Hedef")]
        public string TomorrowGoal { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Uyku Süresi (Saat)")]
        public double SleepHours { get; set; }
    }
}
