using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Employee,HR")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IPositionService _positionService;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly IAnnouncementService _announcementService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IPositionService positionService,
            ILeaveRequestService leaveRequestService,
            IAnnouncementService announcementService,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _positionService = positionService;
            _leaveRequestService = leaveRequestService;
            _announcementService = announcementService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Employee/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId!);

            if (employee == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Employee = employee;
            ViewBag.PendingLeaves = (await _leaveRequestService.GetLeaveRequestsByEmployeeIdAsync(employee.EmployeeId))
                .Count(l => l.Status == LeaveStatus.Pending);

            var announcements = await _announcementService.GetActiveAnnouncementsAsync();
            return View(announcements);
        }

        // GET: /Employee/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = _userManager.GetUserId(User);
            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId!);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: /Employee/EditProfile
        public async Task<IActionResult> EditProfile()
        {
            var userId = _userManager.GetUserId(User);
            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId!);

            if (employee == null)
            {
                return NotFound();
            }

            await LoadDropdownsAsync();
            return View(employee);
        }

        // POST: /Employee/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Employee employee)
        {
            var userId = _userManager.GetUserId(User);
            var existingEmployee = await _employeeService.GetEmployeeByUserIdAsync(userId!);

            if (existingEmployee == null)
            {
                return NotFound();
            }

            // Update only allowed fields (NO profile picture upload for employees)
            existingEmployee.PhoneNumber = employee.PhoneNumber;
            existingEmployee.Address = employee.Address;

            await _employeeService.UpdateEmployeeAsync(existingEmployee);
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        // GET: /Employee/MyLeaves
        public async Task<IActionResult> MyLeaves()
        {
            var userId = _userManager.GetUserId(User);
            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId!);

            if (employee == null)
            {
                return NotFound();
            }

            var leaves = await _leaveRequestService.GetLeaveRequestsByEmployeeIdAsync(employee.EmployeeId);
            return View(leaves);
        }

        // GET: /Employee/FileLeave
        public async Task<IActionResult> FileLeave()
        {
            var userId = _userManager.GetUserId(User);
            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId!);

            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.EmployeeId = employee.EmployeeId;
            return View();
        }

        // POST: /Employee/FileLeave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FileLeave(LeaveRequest leaveRequest)
        {
            if (ModelState.IsValid)
            {
                await _leaveRequestService.CreateLeaveRequestAsync(leaveRequest);
                TempData["Success"] = "Leave request submitted successfully!";
                return RedirectToAction(nameof(MyLeaves));
            }

            return View(leaveRequest);
        }

        // POST: /Employee/CancelLeave/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelLeave(int id)
        {
            await _leaveRequestService.CancelLeaveRequestAsync(id);
            TempData["Success"] = "Leave request cancelled.";
            return RedirectToAction(nameof(MyLeaves));
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