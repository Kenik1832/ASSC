using System.ComponentModel.DataAnnotations;

namespace ASSC.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public Invoice Invoice { get; set; } = null!;

        [Required]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}