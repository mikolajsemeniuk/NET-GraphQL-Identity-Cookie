using System.ComponentModel.DataAnnotations;

namespace app.Models
{
    public class Platform
    {
        [Key]
        public int PlatformId { get; set; }
        [Required]
        public string Name { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}