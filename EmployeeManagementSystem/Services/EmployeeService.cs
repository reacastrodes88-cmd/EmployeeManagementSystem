using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Where(e => e.IsActive)
                .OrderBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<Employee?> GetEmployeeByUserIdAsync(string userId)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            // Soft delete - just set IsActive to false
            employee.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.EmployeeId == id);
        }

        public async Task<bool> EmployeeNumberExistsAsync(string employeeNumber)
        {
            return await _context.Employees.AnyAsync(e => e.EmployeeNumber == employeeNumber);
        }

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Where(e => e.IsActive &&
                    (e.FirstName.Contains(searchTerm) ||
                     e.LastName.Contains(searchTerm) ||
                     e.EmployeeNumber.Contains(searchTerm) ||
                     e.Email.Contains(searchTerm)))
                .OrderBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<int> GetTotalEmployeeCountAsync()
        {
            return await _context.Employees.CountAsync(e => e.IsActive);
        }

        public async Task<string> GenerateEmployeeNumberAsync()
        {
            // Get the last employee number
            var lastEmployee = await _context.Employees
                .OrderByDescending(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastEmployee != null)
            {
                // Extract number from last employee number (e.g., EMP-0001 -> 1)
                var lastNumberStr = lastEmployee.EmployeeNumber.Replace("EMP-", "");
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            // Format as EMP-0001, EMP-0002, etc.
            return $"EMP-{nextNumber:D4}";
        }
    }
}