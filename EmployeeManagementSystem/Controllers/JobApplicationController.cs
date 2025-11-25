using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.Controllers
{
    public class JobApplicationController : Controller
    {
        private readonly IJobApplicationService _jobApplicationService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public JobApplicationController(
            IJobApplicationService jobApplicationService,
            IWebHostEnvironment webHostEnvironment)
        {
            _jobApplicationService = jobApplicationService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /JobApplication/Apply (PUBLIC - No login required)
        [AllowAnonymous]
        public IActionResult Apply()
        {
            return View();
        }

        // POST: /JobApplication/Apply
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(JobApplication application, IFormFile? resume)
        {
            ModelState.Remove("ResumeFilePath");
            ModelState.Remove("Status");
            ModelState.Remove("DateApplied");

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle resume file upload
                    if (resume != null && resume.Length > 0)
                    {
                        // Use absolute path
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "resumes");

                        // Create directory if it doesn't exist
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = $"{application.FirstName}_{application.LastName}_{Guid.NewGuid()}{Path.GetExtension(resume.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await resume.CopyToAsync(fileStream);
                        }

                        application.ResumeFilePath = $"/uploads/resumes/{uniqueFileName}";
                    }

                    application.Status = ApplicationStatus.Pending;
                    application.DateApplied = DateTime.Now;

                    await _jobApplicationService.CreateJobApplicationAsync(application);

                    TempData["Success"] = "Your application has been submitted successfully! We will contact you soon.";
                    return RedirectToAction("ThankYou");
                }
                catch (Exception ex)
                {
                    // Log the error
                    ModelState.AddModelError("", $"An error occurred while submitting your application: {ex.Message}");
                    return View(application);
                }
            }

            return View(application);
        }

        // GET: /JobApplication/ThankYou
        [AllowAnonymous]
        public IActionResult ThankYou()
        {
            return View();
        }

        // GET: /JobApplication/Details/5 (HR Only)
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Details(int id)
        {
            var application = await _jobApplicationService.GetJobApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            return View(application);
        }

        // POST: /JobApplication/UpdateStatus (HR Only)
        [HttpPost]
        [Authorize(Roles = "HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, ApplicationStatus status, string? notes)
        {
            var application = await _jobApplicationService.GetJobApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            application.Status = status;
            application.HRNotes = notes;
            application.DateReviewed = DateTime.Now;

            await _jobApplicationService.UpdateJobApplicationAsync(application);
            TempData["Success"] = $"Application status updated to {status}.";

            return RedirectToAction("Applications", "HR");
        }
    }
}