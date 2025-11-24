using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Models.ViewModels;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "HR")]
    public class HRController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IPositionService _positionService;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly IAnnouncementService _announcementService;
        private readonly IJobApplicationService _jobApplicationService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HRController(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IPositionService positionService,
            ILeaveRequestService leaveRequestService,
            IAnnouncementService announcementService,
            IJobApplicationService jobApplicationService,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _positionService = positionService;
            _leaveRequestService = leaveRequestService;
            _announcementService = announcementService;
            _jobApplicationService = jobApplicationService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /HR/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalEmployees = await _employeeService.GetTotalEmployeeCountAsync();
            ViewBag.PendingLeaves = await _leaveRequestService.GetPendingLeaveCountAsync();
            ViewBag.Departments = (await _departmentService.GetAllDepartmentsAsync()).Count();
            ViewBag.Positions = (await _positionService.GetAllPositionsAsync()).Count();

            var recentEmployees = (await _employeeService.GetAllEmployeesAsync()).Take(5);
            return View(recentEmployees);
        }

        // GET: /HR/Index (Employee List)
        public async Task<IActionResult> Index(string? searchTerm)
        {
            IEnumerable<Employee> employees;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                employees = await _employeeService.SearchEmployeesAsync(searchTerm);
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                employees = await _employeeService.GetAllEmployeesAsync();
            }

            return View(employees);
        }

        // GET: /HR/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View();
        }

        // POST: /HR/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel model, IFormFile? profilePicture)
        {
            if (ModelState.IsValid)
            {
                // Auto-generate Employee Number
                var employeeNumber = await _employeeService.GenerateEmployeeNumberAsync();

                // Create Identity User
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Employee");

                    var employee = new Employee
                    {
                        EmployeeNumber = employeeNumber, // Auto-generated
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Gender = model.Gender,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        DateOfBirth = model.DateOfBirth,
                        DateHired = model.DateHired,
                        DepartmentId = model.DepartmentId,
                        PositionId = model.PositionId,
                        UserId = user.Id,
                        IsActive = true
                    };

                    // Handle profile picture upload
                    if (profilePicture != null && profilePicture.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = $"{employeeNumber}_{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await profilePicture.CopyToAsync(fileStream);
                        }

                        employee.ProfilePicturePath = $"/uploads/profiles/{uniqueFileName}";
                    }

                    await _employeeService.CreateEmployeeAsync(employee);
                    TempData["Success"] = "Employee created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            await LoadDropdownsAsync();
            return View(model);
        }

        // GET: /HR/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            await LoadDropdownsAsync();
            return View(employee);
        }

        // POST: /HR/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee, IFormFile? profilePicture)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Handle profile picture upload
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{employee.EmployeeNumber}_{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(fileStream);
                    }

                    // Delete old profile picture if exists
                    if (!string.IsNullOrEmpty(employee.ProfilePicturePath))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, employee.ProfilePicturePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    employee.ProfilePicturePath = $"/uploads/profiles/{uniqueFileName}";
                }

                await _employeeService.UpdateEmployeeAsync(employee);
                TempData["Success"] = "Employee updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdownsAsync();
            return View(employee);
        }

        // GET: /HR/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: /HR/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            TempData["Success"] = "Employee deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /HR/LeaveRequests
        public async Task<IActionResult> LeaveRequests()
        {
            var leaves = await _leaveRequestService.GetAllLeaveRequestsAsync();
            return View(leaves);
        }

        // POST: /HR/ApproveLeave/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLeave(int id, string? remarks)
        {
            await _leaveRequestService.ApproveLeaveRequestAsync(id, remarks);
            TempData["Success"] = "Leave request approved!";
            return RedirectToAction(nameof(LeaveRequests));
        }

        // POST: /HR/RejectLeave/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectLeave(int id, string? remarks)
        {
            await _leaveRequestService.RejectLeaveRequestAsync(id, remarks);
            TempData["Success"] = "Leave request rejected.";
            return RedirectToAction(nameof(LeaveRequests));
        }

        // GET: /HR/Announcements
        public async Task<IActionResult> Announcements()
        {
            var announcements = await _announcementService.GetAllAnnouncementsAsync();
            return View(announcements);
        }

        // GET: /HR/CreateAnnouncement
        public IActionResult CreateAnnouncement()
        {
            return View();
        }

        // POST: /HR/CreateAnnouncement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAnnouncement(Announcement announcement)
        {
            // Remove validation for fields we'll set manually
            ModelState.Remove("PostedBy");
            ModelState.Remove("DatePosted");

            if (ModelState.IsValid)
            {
                announcement.PostedBy = User.Identity?.Name ?? "HR";
                announcement.DatePosted = DateTime.Now;
                await _announcementService.CreateAnnouncementAsync(announcement);
                TempData["Success"] = "Announcement posted successfully!";
                return RedirectToAction(nameof(Announcements));
            }

            // Log validation errors for debugging
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine($"Validation Error: {error.ErrorMessage}");
            }

            return View(announcement);
        }

        // GET: /HR/EditAnnouncement/5
        public async Task<IActionResult> EditAnnouncement(int id)
        {
            var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }
            return View(announcement);
        }

        // POST: /HR/EditAnnouncement/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnnouncement(int id, Announcement announcement)
        {
            if (id != announcement.AnnouncementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _announcementService.UpdateAnnouncementAsync(announcement);
                TempData["Success"] = "Announcement updated successfully!";
                return RedirectToAction(nameof(Announcements));
            }
            return View(announcement);
        }

        // POST: /HR/DeleteAnnouncement/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            await _announcementService.DeleteAnnouncementAsync(id);
            TempData["Success"] = "Announcement deleted successfully!";
            return RedirectToAction(nameof(Announcements));
        }

        // GET: /HR/Applications - View Job Applications
        public async Task<IActionResult> Applications()
        {
            var applications = await _jobApplicationService.GetAllJobApplicationsAsync();
            return View(applications);
        }

        // GET: /HR/ApplicationDetails/5 - View Application Details
        public async Task<IActionResult> ApplicationDetails(int id)
        {
            var application = await _jobApplicationService.GetJobApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            return View(application);
        }

        // GET: /HR/HireApplicant/5 - Convert applicant to employee
        public async Task<IActionResult> HireApplicant(int id)
        {
            var application = await _jobApplicationService.GetJobApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            // Pre-fill RegisterViewModel with applicant data
            var model = new RegisterViewModel
            {
                FirstName = application.FirstName,
                LastName = application.LastName,
                Email = application.Email,
                PhoneNumber = application.PhoneNumber,
                Address = application.Address,
                DateOfBirth = application.DateOfBirth,
                DateHired = DateTime.Now
            };

            ViewBag.ApplicationId = id;
            await LoadDropdownsAsync();
            return View(model);
        }

        // POST: /HR/HireApplicant
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HireApplicant(int applicationId, RegisterViewModel model, IFormFile? profilePicture)
        {
            if (ModelState.IsValid)
            {
                // Auto-generate Employee Number
                var employeeNumber = await _employeeService.GenerateEmployeeNumberAsync();

                // Create employee account
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Employee");

                    var employee = new Employee
                    {
                        EmployeeNumber = employeeNumber, // Auto-generated
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Gender = model.Gender,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        DateOfBirth = model.DateOfBirth,
                        DateHired = model.DateHired,
                        DepartmentId = model.DepartmentId,
                        PositionId = model.PositionId,
                        UserId = user.Id,
                        IsActive = true
                    };

                    // Handle profile picture
                    if (profilePicture != null && profilePicture.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");
                        Directory.CreateDirectory(uploadsFolder);
                        var uniqueFileName = $"{employeeNumber}_{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await profilePicture.CopyToAsync(fileStream);
                        }
                        employee.ProfilePicturePath = $"/uploads/profiles/{uniqueFileName}";
                    }

                    await _employeeService.CreateEmployeeAsync(employee);

                    // Update application status to Hired
                    var application = await _jobApplicationService.GetJobApplicationByIdAsync(applicationId);
                    if (application != null)
                    {
                        application.Status = ApplicationStatus.Hired;
                        application.DateReviewed = DateTime.Now;
                        await _jobApplicationService.UpdateJobApplicationAsync(application);
                    }

                    TempData["Success"] = "Applicant hired successfully and employee account created!";
                    return RedirectToAction(nameof(Applications));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.ApplicationId = applicationId;
            await LoadDropdownsAsync();
            return View(model);
        }

        private async Task LoadDropdownsAsync()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var positions = await _positionService.GetAllPositionsAsync();

            ViewBag.Departments = new SelectList(departments, "DepartmentId", "DepartmentName");
            ViewBag.Positions = new SelectList(positions, "PositionId", "PositionTitle");
        }
    }
}