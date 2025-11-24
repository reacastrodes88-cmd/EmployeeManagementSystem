using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public interface IJobApplicationService
    {
        Task<IEnumerable<JobApplication>> GetAllJobApplicationsAsync();
        Task<IEnumerable<JobApplication>> GetPendingJobApplicationsAsync();
        Task<JobApplication?> GetJobApplicationByIdAsync(int id);
        Task<JobApplication> CreateJobApplicationAsync(JobApplication application);
        Task<JobApplication> UpdateJobApplicationAsync(JobApplication application);
        Task<bool> DeleteJobApplicationAsync(int id);
        Task<int> GetPendingApplicationCountAsync();
    }
}