using Microsoft.AspNetCore.Identity;

namespace ASSC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? SupplierId { get; set; }
        public int? ContractorId { get; set; }
    }
}