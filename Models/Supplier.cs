using System.ComponentModel.DataAnnotations;

namespace ASSC.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string INN { get; set; } = null!;
        
        public string ContactInfo { get; set; } = null!;
        
        public ICollection<Contract> Contracts { get; set; }
            = new List<Contract>();
    }
}