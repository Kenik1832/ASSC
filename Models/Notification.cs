using System.ComponentModel.DataAnnotations;

namespace ASSC.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        [Required]
        public string Message { get; set; } = null!;

        public bool IsRead { get; set; }

        public DateTime DateCreated { get; set; }
    }
}