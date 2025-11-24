using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }

    public enum LeaveType
    {
        Vacation,
        Sick,
        Emergency,
        Maternity,
        Paternity,
        Unpaid
    }

    public class LeaveRequest
    {
        [Key]
        public int LeaveRequestId { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Leave Type")]
        public LeaveType LeaveType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

        [StringLength(500)]
        [Display(Name = "HR Remarks")]
        public string? Remarks { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Filed")]
        public DateTime DateFiled { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Date Processed")]
        public DateTime? DateProcessed { get; set; }

        // Navigation Property
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        // Computed Property
        [NotMapped]
        public int TotalDays => (EndDate - StartDate).Days + 1;
    }
}