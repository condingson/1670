using Microsoft.AspNetCore.Builder;

namespace demo12.Models
{
    public class JobC
    {
        public List<Job> job { get; set; }
        public List<ApplicationJob> applicationJob { get; set; }
    }
}
