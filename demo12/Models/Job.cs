using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace demo12.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Industry { get; set; }
        public string? Requiered { get; set; }
        public DateTime Deadline { get; set; }
        public double Salary { get; set; }
        public string? DImage { get; set; }
        [NotMapped]
        public IFormFile? Jmage { get; set; }         
        
        [ForeignKey("Users")]
        public string UserId { get; set; }
        public virtual ApplicationUser Users { get; set; }
    }
}
