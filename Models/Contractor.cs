using System.ComponentModel.DataAnnotations;

namespace ASSC.Models
{
    public class Contractor
    {
        public int Id { get; set; }

        [Required(ErrorMessage="Название обязательно")]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage="ИНН обязателен")]
        [StringLength(12)]
        public string INN { get; set; } = null!;

        [StringLength(200)]
        public string ContactInfo { get; set; } = null!;

        public ICollection<Contract> Contracts { get; set; }
            = new List<Contract>();
    }
}