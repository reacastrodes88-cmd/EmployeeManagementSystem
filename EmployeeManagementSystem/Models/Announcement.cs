using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public enum AnnouncementPriority
    {
        Normal,
        Important,
        Urgent
    }

    public class Announcement
    {
        [Key]
        public int AnnouncementId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        [Display(Name = "Announcement Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(2000)]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Priority")]
        public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Required]
        [StringLength(100)]
        [Display(Name = "Posted By")]
        public string PostedBy { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Date Posted")]
        public DateTime DatePosted { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Attachment File Path")]
        public string? AttachmentPath { get; set; }
    }
}