using System.ComponentModel.DataAnnotations;

namespace ASSC.Models
{
    public class Contract
    {
        public int Id { get; set; }

        [Required]
        public string Number { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // FK
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        public int ContractorId { get; set; }
        public Contractor Contractor { get; set; } = null!;

        public ICollection<Invoice> Invoices { get; set; }
            = new List<Invoice>();
    }
}