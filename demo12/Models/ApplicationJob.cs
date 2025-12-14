using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace demo12.Models
{
    public class ApplicationJob
    {
        [Key]
        public int Id { get; set; }
        public int? JobId { get; set; }
        public string? Title { get; set; }
        public string? Introduction { get; set; }
        public string? Reason { get; set; }
        
        [ForeignKey("Users")]
        public string UserId { get; set; }
        public virtual ApplicationUser Users { get; set; }
    }
}
