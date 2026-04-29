using System.ComponentModel.DataAnnotations;

namespace ASSC.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public int ContractId { get; set; }

        public Contract Contract { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime DueDate { get; set; }

        public string Status { get; set; }

        public ICollection<Payment> Payments { get; set; }
            = new List<Payment>();
    }
}