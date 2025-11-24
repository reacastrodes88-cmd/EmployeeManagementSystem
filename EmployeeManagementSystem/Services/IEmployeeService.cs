using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee?> GetEmployeeByUserIdAsync(string userId);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<bool> EmployeeExistsAsync(int id);
        Task<bool> EmployeeNumberExistsAsync(string employeeNumber);
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm);
        Task<int> GetTotalEmployeeCountAsync();
        Task<string> GenerateEmployeeNumberAsync();
    }
}