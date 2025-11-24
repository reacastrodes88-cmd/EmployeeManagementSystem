using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for our models
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed default departments
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, DepartmentName = "Human Resources", Description = "HR Department - Manages recruitment, employee relations, and HR policies", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Department { DepartmentId = 2, DepartmentName = "Information Technology", Description = "IT Department - Handles system infrastructure, development, and support", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Department { DepartmentId = 3, DepartmentName = "Finance & Accounting", Description = "Finance Department - Manages budgets, payroll, and financial reporting", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Department { DepartmentId = 4, DepartmentName = "Marketing & Sales", Description = "Marketing Department - Handles branding, campaigns, and customer relations", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Department { DepartmentId = 5, DepartmentName = "Operations", Description = "Operations Department - Manages daily business operations and logistics", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Department { DepartmentId = 6, DepartmentName = "Customer Service", Description = "Customer Service - Handles customer inquiries and support", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Department { DepartmentId = 7, DepartmentName = "Research & Development", Description = "R&D Department - Focuses on innovation and product development", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Department { DepartmentId = 8, DepartmentName = "Legal & Compliance", Description = "Legal Department - Ensures compliance and handles legal matters", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Department { DepartmentId = 9, DepartmentName = "Quality Assurance", Description = "QA Department - Ensures product and service quality standards", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Department { DepartmentId = 10, DepartmentName = "Administration", Description = "Admin Department - Handles general administration and office management", IsActive = true, DateCreated = new DateTime(2024, 1, 1) }
            );

            // Seed default positions
            modelBuilder.Entity<Position>().HasData(
                new Position { PositionId = 1, PositionTitle = "Chief Executive Officer (CEO)", Description = "Top executive responsible for overall company operations", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 2, PositionTitle = "Chief Technology Officer (CTO)", Description = "Oversees technology strategy and development", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 3, PositionTitle = "Chief Financial Officer (CFO)", Description = "Manages financial planning and operations", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 4, PositionTitle = "Department Manager", Description = "Manages department operations and team", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 5, PositionTitle = "Team Leader", Description = "Leads a team and coordinates daily activities", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 6, PositionTitle = "Senior Developer", Description = "Experienced software developer with leadership responsibilities", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 7, PositionTitle = "Software Developer", Description = "Develops and maintains software applications", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 8, PositionTitle = "Junior Developer", Description = "Entry-level developer learning and assisting senior developers", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 9, PositionTitle = "Business Analyst", Description = "Analyzes business processes and requirements", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 10, PositionTitle = "Project Manager", Description = "Plans and oversees project execution", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 11, PositionTitle = "HR Specialist", Description = "Handles recruitment and employee relations", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 12, PositionTitle = "Accountant", Description = "Manages financial records and reporting", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 13, PositionTitle = "Marketing Specialist", Description = "Creates and executes marketing campaigns", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 14, PositionTitle = "Sales Representative", Description = "Sells products and services to customers", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 15, PositionTitle = "Customer Support Representative", Description = "Provides customer service and support", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 16, PositionTitle = "Quality Assurance Tester", Description = "Tests software and ensures quality standards", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 17, PositionTitle = "Administrative Assistant", Description = "Provides administrative support to the team", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 18, PositionTitle = "Intern", Description = "Trainee learning on the job", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 19, PositionTitle = "IT Support Specialist", Description = "Provides technical support and troubleshooting", IsActive = true, DateCreated = new DateTime(2024, 1, 1) },
                new Position { PositionId = 20, PositionTitle = "Data Analyst", Description = "Analyzes data and provides insights", IsActive = true, DateCreated = new DateTime(2024, 1, 1) }
            );

            // Configure relationships
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure unique constraint for EmployeeNumber
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeNumber)
                .IsUnique();
        }
    }
}