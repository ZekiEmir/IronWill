using System.ComponentModel.DataAnnotations;

namespace IronWill.Models
{
    public class Project
    {
        public int Id { get; set; }
        
        public string ClientName { get; set; }
        
        public string ProjectName { get; set; }
        
        public decimal ContractAmount { get; set; }
        
        public decimal PaidAmount { get; set; }
        
        public DateTime Deadline { get; set; }
        
        public string Status { get; set; } // "Pending", "Active", "Completed"
        
        public int ProgressPercentage { get; set; }
    }
}
