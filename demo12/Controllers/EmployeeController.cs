using demo12.Data;
using demo12.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using Newtonsoft.Json;

namespace demo12.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize(Roles = "Employee")]
        public IActionResult Index()
        {
            var listJob = _context.Jobs.ToList();
            var user = _userManager.GetUserId(User);
            ViewBag.UserId = user;
            return View(listJob);
        }
        
        [HttpGet]
        public IActionResult CreateJob()
        {
            var user = _userManager.GetUserId(User);
            if (user != null)
            {
                ViewBag.UserId = user;
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        [NonAction]
        private string UploadedFile(Job model)
        {
            string uniqueFileName = null;
            if (model.Jmage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Jmage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Jmage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        [HttpPost]
        public IActionResult CreateJob(Job model)
        {
            string uniqueFileName = UploadedFile(model);
            Job job = new Job
            {
                Title = model.Title,
                Location = model.Location,
                Industry = model.Industry,
                DImage = uniqueFileName,
                Description = model.Description,
                Requiered = model.Requiered,
                Deadline = model.Deadline,
                Salary = model.Salary,
                UserId = model.UserId,
            };
            _context.Jobs.Add(job);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public IActionResult DetailJob(int? id)
        {
            ViewBag.JobId = id;
            var job = _context.Jobs.Find(id);
            var jobs = _context.Jobs.ToList();
            var viewmodel = new JobV
            {
                JobList = jobs,
                job = job
            };
            return View(viewmodel);
        }
        [HttpGet]
        public IActionResult EditJob(int? id)
        {
            if (id != null)
            {
                var editJob = _context.Jobs.Find(id);
                return View(editJob);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult EditJob(Job model)
        {
            var jobToUpdate = _context.Jobs.FirstOrDefault(j => j.Id == model.Id);
            if (jobToUpdate == null)
            {
                return NotFound();
            }
            string uniqueFileName = jobToUpdate.DImage;
            // Xác định đường dẫn đến thư mục chứa ảnh cũ
            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", jobToUpdate.DImage);

            if (model.Jmage != null)
            {
                // Kiểm tra xem tập tin ảnh cũ có tồn tại không và sau đó xóa nó
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                uniqueFileName = UploadedFile(model);
            }
            jobToUpdate.Title = model.Title;
            jobToUpdate.Location = model.Location;
            jobToUpdate.Industry = model.Industry;
            jobToUpdate.DImage = uniqueFileName;
            jobToUpdate.Description = model.Description;
            jobToUpdate.Requiered = model.Requiered;
            jobToUpdate.Deadline = model.Deadline;
            jobToUpdate.Salary = model.Salary;
            jobToUpdate.UserId = model.UserId;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult DeleteJob(int? id)
        {
            if (id != null)
            {
                var deleteJob = _context.Jobs.Find(id);
                return View(deleteJob);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult DeleteJob(Job model)
        {
            var jobToDelete = _context.Jobs.FirstOrDefault(j => j.Id == model.Id);
            if (jobToDelete == null)
            {
                return NotFound();
            }
            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", jobToDelete.DImage);

            // Kiểm tra xem tập tin ảnh cũ có tồn tại không và sau đó xóa nó
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _context.Jobs.Remove(jobToDelete);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult ReplyCV(int? id)
        {
            ViewBag.JobId = id;
            var applicationJob = _context.ApplcationJobs.Include(x => x.Users).ToList();
            return View(applicationJob);
        }
        [HttpPost]
        public IActionResult DetailCV(int jobId, int jobAppId)
        {
            ViewBag.JobId = jobId;
            var applicationJob = _context.ApplcationJobs.Find(jobAppId);
            return View(applicationJob);
        }
        [HttpPost]
        public IActionResult AcceptJob(int jobId, int jobAppId)
        {
            var model = _context.ApplcationJobs.Find(jobAppId);
            model.Reason = "Accepted";
            _context.SaveChanges();
            int id = jobId;
            return RedirectToAction(nameof(ReplyCV), new { id });
        }
        [HttpPost]
        public IActionResult RejectJob(int jobId, int jobAppId)
        {
            var model = _context.ApplcationJobs.Find(jobAppId);
            model.Reason = "Rejected";
            _context.SaveChanges();
            int id = jobId;
            return RedirectToAction(nameof(ReplyCV), new { id });
        }
    }
    
}
