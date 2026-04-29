using System.ComponentModel.DataAnnotations;

namespace ASSC.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string INN { get; set; }
        
        public string ContactInfo { get; set; }
        
        public ICollection<Contract> Contracts { get; set; }
            = new List<Contract>();
    }
}