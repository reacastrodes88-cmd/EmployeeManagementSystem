using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync()
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .OrderByDescending(l => l.DateFiled)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId)
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .Where(l => l.EmployeeId == employeeId)
                .OrderByDescending(l => l.DateFiled)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetPendingLeaveRequestsAsync()
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .Where(l => l.Status == LeaveStatus.Pending)
                .OrderBy(l => l.DateFiled)
                .ToListAsync();
        }

        public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.LeaveRequestId == id);
        }

        public async Task<LeaveRequest> CreateLeaveRequestAsync(LeaveRequest leaveRequest)
        {
            leaveRequest.DateFiled = DateTime.Now;
            leaveRequest.Status = LeaveStatus.Pending;
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();
            return leaveRequest;
        }

        public async Task<LeaveRequest> UpdateLeaveRequestAsync(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Update(leaveRequest);
            await _context.SaveChangesAsync();
            return leaveRequest;
        }

        public async Task<bool> ApproveLeaveRequestAsync(int id, string? remarks)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave == null) return false;

            leave.Status = LeaveStatus.Approved;
            leave.Remarks = remarks;
            leave.DateProcessed = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectLeaveRequestAsync(int id, string? remarks)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave == null) return false;

            leave.Status = LeaveStatus.Rejected;
            leave.Remarks = remarks;
            leave.DateProcessed = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelLeaveRequestAsync(int id)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave == null) return false;

            leave.Status = LeaveStatus.Cancelled;
            leave.DateProcessed = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetPendingLeaveCountAsync()
        {
            return await _context.LeaveRequests.CountAsync(l => l.Status == LeaveStatus.Pending);
        }
    }
}