using demo12.Data;
using demo12.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var jobs = _context.Jobs.ToList();
            return View(jobs);
        }

        [HttpGet]
        public IActionResult DetailJob(int? id)
        {
            var job = _context.Jobs.Find(id);
            var jobs = _context.Jobs.ToList();
            var viewmodel = new JobV
            {
                JobList = jobs,
                job = job
            };
            return View(viewmodel);
        }

        [HttpPost]
        public IActionResult Filter(string SearchJob, string SearchLocation)
        {
            var jobSearch = _context.Jobs.ToList();
            if (!String.IsNullOrEmpty(SearchJob) && String.IsNullOrEmpty(SearchLocation))
            {
                jobSearch = jobSearch.Where(n => n.Title.Contains(SearchJob)).ToList();
            }
            else if (!String.IsNullOrEmpty(SearchLocation) && String.IsNullOrEmpty(SearchJob))
            {
                jobSearch = jobSearch.Where(n => n.Location.Contains(SearchLocation)).ToList();
            }
            else if (!String.IsNullOrEmpty(SearchLocation) && !String.IsNullOrEmpty(SearchJob))
            {
                jobSearch = jobSearch.Where(n => n.Title.Contains(SearchJob) && n.Location.Contains(SearchLocation)).ToList();
            }
            else
            {
                return View(jobSearch);
            }
            return View(jobSearch);
        }

        [NonAction]
        private void LoadJob()
        {
            var jobs = _context.Jobs.ToList();
            ViewBag.Jobs = new SelectList(jobs, "Id", "JobTitle");
        }

        //[HttpGet]
        //public IActionResult CreateCV()
        //{
        //    var user = _userManager.GetUserId(User);
        //    ViewBag.UserId = user;
        //    LoadJob();
        //    return View();
        //}

        [HttpGet]
        [Authorize(Roles = "Jobseeker")]
        public IActionResult CreateCV(int? id)
        {
            var user = _userManager.GetUserId(User);
            var jobId = _context.Jobs.Find(id);
            ViewBag.JobId = jobId.Id;
            ViewBag.UserId = user;
            LoadJob();
            return View();
        }

        [HttpPost]
        public IActionResult CreateCV(ApplicationJob model)
        {
            
            ApplicationJob applicationJob = new ApplicationJob
            {
                Title = model.Title,
                Reason = model.Reason,
                Introduction = model.Introduction,
                JobId = model.JobId,
                UserId = model.UserId,
            };
            _context.ApplcationJobs.Add(applicationJob);
            _context.SaveChanges();
            return RedirectToAction(nameof(CV));
        }

        [HttpGet]
        [Authorize(Roles = "Jobseeker")]
        public IActionResult CV()
        {
            var applicationJob = _context.ApplcationJobs.ToList();
            var job = _context.Jobs.ToList();
            var viewModel = new JobC
            {
                job = job,
                applicationJob = applicationJob,
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult DetailCV(int? id)
        {
            var applicationJob = _context.ApplcationJobs.Find(id);
            return View(applicationJob);
        }

        [HttpGet]
        public IActionResult DeleteJobApp(int? id)
        {
            var model = _context.ApplcationJobs.Find(id);
            var applicationJob = _context.ApplcationJobs.FirstOrDefault(j => j.Id == model.Id);
            if (applicationJob == null)
            {
                return NotFound();
            }            
            _context.ApplcationJobs.Remove(applicationJob);
            _context.SaveChanges();
            return RedirectToAction(nameof(CV));
        }

        [HttpGet]
        public IActionResult EditCV(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationJob = _context.ApplcationJobs.Find(id);
            if (applicationJob == null)
            {
                return NotFound();
            }

            var user = _userManager.GetUserId(User);
            ViewBag.UserId = user;
            ViewBag.JobId = applicationJob.JobId;
            LoadJob();
            return View(applicationJob);
        }

        // POST
        [HttpPost]
        public IActionResult EditCV(int id, ApplicationJob model)
        {
            var applicationJob = _context.ApplcationJobs.FirstOrDefault(j => j.Id == id);
            if (applicationJob == null)
            {
                return NotFound();
            }          
            applicationJob.Title = model.Title;
            applicationJob.Reason = model.Reason;
            applicationJob.JobId = model.JobId;
            applicationJob.Introduction = model.Introduction;
            applicationJob.UserId = model.UserId;

            _context.SaveChanges();
            return RedirectToAction(nameof(CV));
        }

    }
}
