using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync();
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<LeaveRequest>> GetPendingLeaveRequestsAsync();
        Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id);
        Task<LeaveRequest> CreateLeaveRequestAsync(LeaveRequest leaveRequest);
        Task<LeaveRequest> UpdateLeaveRequestAsync(LeaveRequest leaveRequest);
        Task<bool> ApproveLeaveRequestAsync(int id, string? remarks);
        Task<bool> RejectLeaveRequestAsync(int id, string? remarks);
        Task<bool> CancelLeaveRequestAsync(int id);
        Task<int> GetPendingLeaveCountAsync();
    }
}