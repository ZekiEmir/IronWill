using System.ComponentModel.DataAnnotations;

namespace IronWill.Models
{
    public class Project
    {
        public int Id { get; set; }
        
        public string ClientName { get; set; }
        
        public string ProjectName { get; set; }
        
        public decimal AgreedPrice { get; set; }
        
        public decimal PaidAmount { get; set; }
        
        public string Status { get; set; } // "Pending", "Active", "Completed"
        
        public int ProgressPercentage { get; set; }
    }
}
