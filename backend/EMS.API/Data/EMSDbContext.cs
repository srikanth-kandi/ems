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
    }
}
