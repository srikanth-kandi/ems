using EMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Data;

public class EMSDbContext : DbContext
{
    public EMSDbContext(DbContextOptions<EMSDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<PerformanceMetric> PerformanceMetrics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Employee configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.DepartmentId);
            entity.HasIndex(e => e.DateOfJoining);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.FirstName, e.LastName });
            entity.HasIndex(e => new { e.DepartmentId, e.IsActive });
            entity.HasIndex(e => new { e.DateOfJoining, e.IsActive });
            
            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Employees)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Description).HasMaxLength(500);
            entity.Property(d => d.ManagerName).HasMaxLength(100);
            
            entity.HasIndex(d => d.Name).IsUnique();
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).HasMaxLength(50).HasDefaultValue("User");
            
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Attendance configuration
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Notes).HasMaxLength(500);
            
            entity.HasIndex(a => a.EmployeeId);
            entity.HasIndex(a => a.Date);
            entity.HasIndex(a => new { a.EmployeeId, a.Date });
            entity.HasIndex(a => new { a.Date, a.CheckInTime });
            entity.HasIndex(a => a.CheckOutTime);
            
            entity.HasOne(a => a.Employee)
                  .WithMany(e => e.Attendances)
                  .HasForeignKey(a => a.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // PerformanceMetric configuration
        modelBuilder.Entity<PerformanceMetric>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Comments).HasMaxLength(1000);
            entity.Property(p => p.Goals).HasMaxLength(100);
            entity.Property(p => p.Achievements).HasMaxLength(100);
            entity.Property(p => p.PerformanceScore).HasColumnType("decimal(5,2)");
            
            entity.HasIndex(p => p.EmployeeId);
            entity.HasIndex(p => new { p.EmployeeId, p.Year, p.Quarter });
            entity.HasIndex(p => p.PerformanceScore);
            entity.HasIndex(p => new { p.Year, p.Quarter });
            
            entity.HasOne(p => p.Employee)
                  .WithMany(e => e.PerformanceMetrics)
                  .HasForeignKey(p => p.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Departments
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Human Resources", Description = "Manages employee relations, recruitment, and benefits", ManagerName = "Sarah Johnson", CreatedAt = DateTime.UtcNow },
            new Department { Id = 2, Name = "Information Technology", Description = "Handles all technology infrastructure and software development", ManagerName = "Michael Chen", CreatedAt = DateTime.UtcNow },
            new Department { Id = 3, Name = "Finance", Description = "Manages financial planning, accounting, and budgeting", ManagerName = "Robert Williams", CreatedAt = DateTime.UtcNow },
            new Department { Id = 4, Name = "Marketing", Description = "Responsible for brand management and customer acquisition", ManagerName = "Emily Davis", CreatedAt = DateTime.UtcNow },
            new Department { Id = 5, Name = "Sales", Description = "Handles customer relationships and revenue generation", ManagerName = "David Martinez", CreatedAt = DateTime.UtcNow },
            new Department { Id = 6, Name = "Operations", Description = "Manages day-to-day business operations and logistics", ManagerName = "Lisa Anderson", CreatedAt = DateTime.UtcNow },
            new Department { Id = 7, Name = "Customer Support", Description = "Provides customer service and technical support", ManagerName = "James Wilson", CreatedAt = DateTime.UtcNow },
            new Department { Id = 8, Name = "Research & Development", Description = "Conducts product research and innovation", ManagerName = "Dr. Jennifer Taylor", CreatedAt = DateTime.UtcNow },
            new Department { Id = 9, Name = "Legal", Description = "Handles legal compliance and contract management", ManagerName = "Attorney Mark Brown", CreatedAt = DateTime.UtcNow },
            new Department { Id = 10, Name = "Quality Assurance", Description = "Ensures product and service quality standards", ManagerName = "Patricia Garcia", CreatedAt = DateTime.UtcNow }
        );

        // Seed default users
        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                Id = 1, 
                Username = "admin", 
                Email = "admin@ems.com", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), 
                Role = "Admin", 
                IsActive = true, 
                CreatedAt = DateTime.UtcNow 
            },
            new User 
            { 
                Id = 2, 
                Username = "hr_manager", 
                Email = "hr@ems.com", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("hr123"), 
                Role = "HR", 
                IsActive = true, 
                CreatedAt = DateTime.UtcNow 
            },
            new User 
            { 
                Id = 3, 
                Username = "manager", 
                Email = "manager@ems.com", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"), 
                Role = "Manager", 
                IsActive = true, 
                CreatedAt = DateTime.UtcNow 
            }
        );
    }
}
