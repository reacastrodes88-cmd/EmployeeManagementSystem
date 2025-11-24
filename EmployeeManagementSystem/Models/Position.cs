using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Position
    {
        [Key]
        public int PositionId { get; set; }

        [Required(ErrorMessage = "Position title is required")]
        [StringLength(100)]
        [Display(Name = "Position Title")]
        public string PositionTitle { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [DataType(DataType.Date)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Navigation Property
        public virtual ICollection<Employee>? Employees { get; set; }
    }
}