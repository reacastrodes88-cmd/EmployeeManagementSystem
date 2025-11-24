using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public enum ApplicationStatus
    {
        Pending,
        UnderReview,
        Shortlisted,
        Interview,
        Hired,
        Rejected
    }

    public class JobApplication
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Applying For Position")]
        public string ApplyingForPosition { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Highest Educational Attainment")]
        public string? Education { get; set; }

        [StringLength(200)]
        [Display(Name = "Previous Company")]
        public string? PreviousCompany { get; set; }

        [Display(Name = "Years of Experience")]
        public int? YearsOfExperience { get; set; }

        [StringLength(1000)]
        [Display(Name = "Skills (comma-separated)")]
        public string? Skills { get; set; }

        [Required]
        [StringLength(2000)]
        [Display(Name = "Cover Letter / Why do you want to work here?")]
        public string CoverLetter { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Resume/CV File Path")]
        public string? ResumeFilePath { get; set; }

        [Display(Name = "Application Status")]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        [DataType(DataType.Date)]
        [Display(Name = "Date Applied")]
        public DateTime DateApplied { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Date Reviewed")]
        public DateTime? DateReviewed { get; set; }

        [StringLength(500)]
        [Display(Name = "HR Notes")]
        public string? HRNotes { get; set; }

        // Computed Property
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}