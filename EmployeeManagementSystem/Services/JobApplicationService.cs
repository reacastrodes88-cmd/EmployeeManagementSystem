using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly ApplicationDbContext _context;

        public JobApplicationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobApplication>> GetAllJobApplicationsAsync()
        {
            return await _context.JobApplications
                .OrderByDescending(j => j.DateApplied)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetPendingJobApplicationsAsync()
        {
            return await _context.JobApplications
                .Where(j => j.Status == ApplicationStatus.Pending)
                .OrderBy(j => j.DateApplied)
                .ToListAsync();
        }

        public async Task<JobApplication?> GetJobApplicationByIdAsync(int id)
        {
            return await _context.JobApplications
                .FirstOrDefaultAsync(j => j.ApplicationId == id);
        }

        public async Task<JobApplication> CreateJobApplicationAsync(JobApplication application)
        {
            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<JobApplication> UpdateJobApplicationAsync(JobApplication application)
        {
            _context.JobApplications.Update(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<bool> DeleteJobApplicationAsync(int id)
        {
            var application = await _context.JobApplications.FindAsync(id);
            if (application == null) return false;

            _context.JobApplications.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetPendingApplicationCountAsync()
        {
            return await _context.JobApplications
                .CountAsync(j => j.Status == ApplicationStatus.Pending);
        }
    }
}