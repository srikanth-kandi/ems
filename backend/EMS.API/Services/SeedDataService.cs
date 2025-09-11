using EMS.API.Data;
using EMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Services;

public class SeedDataService
{
    private readonly EMSDbContext _context;
    private readonly Random _random = new Random();

    public SeedDataService(EMSDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllDataAsync()
    {
        await SeedDepartmentsAsync();
        await SeedEmployeesAsync();
        await SeedAttendanceAsync();
        await SeedPerformanceMetricsAsync();
        await SeedUsersAsync();
    }

    public async Task ClearAllDataAsync()
    {
        // Clear data in the correct order to respect foreign key constraints
        // First clear dependent tables
        _context.Attendances.RemoveRange(_context.Attendances);
        _context.PerformanceMetrics.RemoveRange(_context.PerformanceMetrics);
        await _context.SaveChangesAsync();

        // Then clear independent tables
        _context.Employees.RemoveRange(_context.Employees);
        _context.Departments.RemoveRange(_context.Departments);
        _context.Users.RemoveRange(_context.Users);
        await _context.SaveChangesAsync();

        // Reset identity columns to start from 1 (MySQL syntax)
        try
        {
            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Attendances AUTO_INCREMENT = 1");
            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE PerformanceMetrics AUTO_INCREMENT = 1");
            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Employees AUTO_INCREMENT = 1");
            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Departments AUTO_INCREMENT = 1");
            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Users AUTO_INCREMENT = 1");
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the operation
            // Some databases might not support AUTO_INCREMENT reset or might have different syntax
            Console.WriteLine($"Warning: Could not reset AUTO_INCREMENT values: {ex.Message}");
        }
    }

    public async Task ReseedAllDataAsync()
    {
        try
        {
            await ClearAllDataAsync();
            
            // Verify that all data has been cleared
            var remainingEmployees = await _context.Employees.CountAsync();
            var remainingDepartments = await _context.Departments.CountAsync();
            var remainingUsers = await _context.Users.CountAsync();
            
            if (remainingEmployees > 0 || remainingDepartments > 0 || remainingUsers > 0)
            {
                throw new InvalidOperationException($"Data clearing incomplete. Remaining: {remainingEmployees} employees, {remainingDepartments} departments, {remainingUsers} users");
            }
            
            await SeedAllDataAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Reseeding failed: {ex.Message}", ex);
        }
    }

    public async Task<int> GetDepartmentCountAsync() => await _context.Departments.CountAsync();
    public async Task<int> GetEmployeeCountAsync() => await _context.Employees.CountAsync();
    public async Task<int> GetAttendanceCountAsync() => await _context.Attendances.CountAsync();
    public async Task<int> GetPerformanceMetricCountAsync() => await _context.PerformanceMetrics.CountAsync();
    public async Task<int> GetUserCountAsync() => await _context.Users.CountAsync();

    private async Task SeedDepartmentsAsync()
    {
        if (await _context.Departments.AnyAsync())
            return;

        var departments = new List<Department>
        {
            new Department { Name = "Human Resources", Description = "Manages employee relations, recruitment, and benefits", ManagerName = "Sarah Johnson", CreatedAt = DateTime.UtcNow },
            new Department { Name = "Information Technology", Description = "Handles all technology infrastructure and software development", ManagerName = "Michael Chen", CreatedAt = DateTime.UtcNow },
            new Department { Name = "Finance", Description = "Manages financial planning, accounting, and budgeting", ManagerName = "Robert Williams", CreatedAt = DateTime.UtcNow },
            new Department { Name = "Marketing", Description = "Responsible for brand management and customer acquisition", ManagerName = "Emily Davis", CreatedAt = DateTime.UtcNow },
            new Department { Name = "Sales", Description = "Handles customer relationships and revenue generation", ManagerName = "David Martinez", CreatedAt = DateTime.UtcNow },
            new Department { Name = "Operations", Description = "Manages day-to-day business operations and logistics", ManagerName = "Lisa Anderson", CreatedAt = DateTime.UtcNow },
            new Department { Name = "Customer Support", Description = "Provides customer service and technical support", ManagerName = "James Wilson", CreatedAt = DateTime.UtcNow },
            new Department { Name = "Research & Development", Description = "Conducts product research and innovation", ManagerName = "Dr. Jennifer Taylor", CreatedAt = DateTime.UtcNow },
            new Department { Name = "Legal", Description = "Handles legal compliance and contract management", ManagerName = "Attorney Mark Brown", CreatedAt = DateTime.UtcNow },
            new Department { Name = "Quality Assurance", Description = "Ensures product and service quality standards", ManagerName = "Patricia Garcia", CreatedAt = DateTime.UtcNow }
        };

        _context.Departments.AddRange(departments);
        await _context.SaveChangesAsync();
    }

    private async Task SeedEmployeesAsync()
    {
        if (await _context.Employees.AnyAsync())
            return;

        var departments = await _context.Departments.ToListAsync();
        var employees = new List<Employee>();

        // First names and last names for variety
        var firstNames = new[]
        {
            "John", "Jane", "Michael", "Sarah", "David", "Emily", "Robert", "Lisa", "James", "Jennifer",
            "William", "Patricia", "Richard", "Linda", "Charles", "Barbara", "Joseph", "Elizabeth", "Thomas", "Jessica",
            "Christopher", "Susan", "Daniel", "Karen", "Paul", "Nancy", "Mark", "Betty", "Donald", "Helen",
            "Steven", "Sandra", "Andrew", "Donna", "Joshua", "Carol", "Kenneth", "Ruth", "Kevin", "Sharon",
            "Brian", "Michelle", "George", "Laura", "Timothy", "Sarah", "Ronald", "Kimberly", "Jason", "Deborah"
        };

        var lastNames = new[]
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
            "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
            "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
            "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts"
        };

        var positions = new[]
        {
            "Software Engineer", "Senior Software Engineer", "Lead Developer", "Project Manager", "Product Manager",
            "Business Analyst", "Data Analyst", "UX Designer", "UI Designer", "DevOps Engineer",
            "System Administrator", "Database Administrator", "Network Engineer", "Security Specialist", "QA Engineer",
            "HR Manager", "HR Specialist", "Recruiter", "Training Coordinator", "Benefits Administrator",
            "Financial Analyst", "Accountant", "Controller", "Budget Analyst", "Tax Specialist",
            "Marketing Manager", "Marketing Specialist", "Content Creator", "Social Media Manager", "Brand Manager",
            "Sales Manager", "Sales Representative", "Account Executive", "Business Development", "Customer Success",
            "Operations Manager", "Operations Analyst", "Supply Chain Manager", "Logistics Coordinator", "Process Improvement",
            "Customer Support Manager", "Support Specialist", "Technical Support", "Help Desk", "Customer Success Manager",
            "Research Scientist", "Research Engineer", "Innovation Manager", "Patent Attorney", "Legal Counsel",
            "Quality Manager", "Quality Engineer", "Compliance Officer", "Auditor", "Risk Manager"
        };

        var baseSalaryRanges = new Dictionary<string, (decimal min, decimal max)>
        {
            ["Human Resources"] = (45000, 120000),
            ["Information Technology"] = (60000, 180000),
            ["Finance"] = (50000, 150000),
            ["Marketing"] = (40000, 130000),
            ["Sales"] = (35000, 200000),
            ["Operations"] = (45000, 140000),
            ["Customer Support"] = (35000, 80000),
            ["Research & Development"] = (70000, 200000),
            ["Legal"] = (80000, 250000),
            ["Quality Assurance"] = (50000, 120000)
        };

        var usedEmails = new HashSet<string>();

        for (int i = 0; i < 200; i++)
        {
            var firstName = firstNames[_random.Next(firstNames.Length)];
            var lastName = lastNames[_random.Next(lastNames.Length)];
            var department = departments[_random.Next(departments.Count)];
            var position = positions[_random.Next(positions.Length)];
            
            var salaryRange = baseSalaryRanges[department.Name];
            var salary = Math.Round((decimal)_random.NextDouble() * (salaryRange.max - salaryRange.min) + salaryRange.min, 2);

            var dateOfBirth = DateTime.Now.AddYears(-_random.Next(22, 65)).AddDays(-_random.Next(0, 365));
            var dateOfJoining = DateTime.Now.AddDays(-_random.Next(30, 3650)); // Last 10 years

            // Generate unique email
            var baseEmail = $"{firstName.ToLower()}.{lastName.ToLower()}@company.com";
            var email = baseEmail;
            var counter = 1;
            
            while (usedEmails.Contains(email))
            {
                email = $"{firstName.ToLower()}.{lastName.ToLower()}{counter}@company.com";
                counter++;
            }
            usedEmails.Add(email);

            var employee = new Employee
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = GeneratePhoneNumber(),
                Address = GenerateAddress(),
                DateOfBirth = dateOfBirth,
                DateOfJoining = dateOfJoining,
                Position = position,
                Salary = salary,
                DepartmentId = department.Id,
                IsActive = _random.NextDouble() > 0.05, // 95% active
                CreatedAt = DateTime.UtcNow
            };

            employees.Add(employee);
        }

        _context.Employees.AddRange(employees);
        await _context.SaveChangesAsync();
    }

    private async Task SeedAttendanceAsync()
    {
        if (await _context.Attendances.AnyAsync())
            return;

        var employees = await _context.Employees.Where(e => e.IsActive).ToListAsync();
        var attendances = new List<Attendance>();

        foreach (var employee in employees)
        {
            // Generate attendance for the last 90 days
            for (int i = 0; i < 90; i++)
            {
                var date = DateTime.Today.AddDays(-i);
                
                // Skip weekends (Saturday = 6, Sunday = 0)
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                // 95% attendance rate
                if (_random.NextDouble() > 0.05)
                {
                    var checkInTime = date.AddHours(8).AddMinutes(_random.Next(-30, 60)); // 7:30 AM to 9:00 AM
                    var checkOutTime = date.AddHours(17).AddMinutes(_random.Next(-60, 120)); // 4:00 PM to 7:00 PM
                    
                    // Some employees work overtime
                    if (_random.NextDouble() > 0.7)
                    {
                        checkOutTime = checkOutTime.AddHours(_random.Next(1, 4));
                    }

                    var totalHours = checkOutTime - checkInTime;

                    var attendance = new Attendance
                    {
                        EmployeeId = employee.Id,
                        CheckInTime = checkInTime,
                        CheckOutTime = checkOutTime,
                        TotalHours = totalHours,
                        Date = date,
                        Notes = _random.NextDouble() > 0.9 ? GenerateAttendanceNote() : null,
                        CreatedAt = DateTime.UtcNow
                    };

                    attendances.Add(attendance);
                }
            }
        }

        _context.Attendances.AddRange(attendances);
        await _context.SaveChangesAsync();
    }

    private async Task SeedPerformanceMetricsAsync()
    {
        if (await _context.PerformanceMetrics.AnyAsync())
            return;

        var employees = await _context.Employees.Where(e => e.IsActive).ToListAsync();
        var performanceMetrics = new List<PerformanceMetric>();

        foreach (var employee in employees)
        {
            // Generate performance metrics for the last 2 years
            for (int year = DateTime.Now.Year - 1; year <= DateTime.Now.Year; year++)
            {
                for (int quarter = 1; quarter <= 4; quarter++)
                {
                    var score = _random.Next(60, 101); // 60-100 range
                    var comments = GeneratePerformanceComment(score);
                    var goals = GeneratePerformanceGoals();
                    var achievements = GenerateAchievements(score);

                    var metric = new PerformanceMetric
                    {
                        EmployeeId = employee.Id,
                        Year = year,
                        Quarter = quarter,
                        PerformanceScore = score,
                        Comments = comments,
                        Goals = goals,
                        Achievements = achievements,
                        CreatedAt = DateTime.UtcNow
                    };

                    performanceMetrics.Add(metric);
                }
            }
        }

        _context.PerformanceMetrics.AddRange(performanceMetrics);
        await _context.SaveChangesAsync();
    }

    private async Task SeedUsersAsync()
    {
        if (await _context.Users.AnyAsync())
            return;

        var users = new List<User>
        {
            new User
            {
                Username = "admin",
                Email = "admin@ems.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "hr_manager",
                Email = "hr@ems.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("hr123"),
                Role = "HR",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "manager",
                Email = "manager@ems.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                Role = "Manager",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
    }

    private string GeneratePhoneNumber()
    {
        return $"{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}";
    }

    private string GenerateAddress()
    {
        var streets = new[] { "Main St", "Oak Ave", "Pine Rd", "Elm St", "Cedar Blvd", "Maple Dr", "First St", "Second Ave" };
        var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego" };
        var states = new[] { "NY", "CA", "IL", "TX", "AZ", "PA", "TX", "CA" };

        return $"{_random.Next(100, 9999)} {streets[_random.Next(streets.Length)]}, {cities[_random.Next(cities.Length)]}, {states[_random.Next(states.Length)]} {_random.Next(10000, 99999)}";
    }

    private string GenerateAttendanceNote()
    {
        var notes = new[]
        {
            "Late due to traffic",
            "Left early for doctor appointment",
            "Worked from home in the morning",
            "Overtime for project deadline",
            "Team meeting ran late",
            "Client call extended hours",
            "Training session",
            "Conference attendance"
        };
        return notes[_random.Next(notes.Length)];
    }

    private string GeneratePerformanceComment(int score)
    {
        if (score >= 90)
            return "Exceptional performance with outstanding results and leadership qualities.";
        else if (score >= 80)
            return "Strong performance with consistent delivery and positive impact.";
        else if (score >= 70)
            return "Good performance with room for improvement in specific areas.";
        else
            return "Performance below expectations, requires improvement and support.";
    }

    private string GeneratePerformanceGoals()
    {
        var goals = new[]
        {
            "Improve technical skills in cloud technologies",
            "Enhance leadership and team management abilities",
            "Complete professional certification program",
            "Increase project delivery efficiency by 20%",
            "Develop better communication skills",
            "Learn new programming languages",
            "Improve customer satisfaction scores",
            "Reduce project costs by 15%"
        };
        return goals[_random.Next(goals.Length)];
    }

    private string GenerateAchievements(int score)
    {
        if (score >= 90)
        {
            var achievements = new[]
            {
                "Led successful project delivery ahead of schedule",
                "Mentored 5 junior team members",
                "Implemented cost-saving initiative saving $50K",
                "Received customer excellence award",
                "Completed advanced certification program"
            };
            return achievements[_random.Next(achievements.Length)];
        }
        else if (score >= 80)
        {
            var achievements = new[]
            {
                "Completed major project on time",
                "Improved team productivity by 15%",
                "Resolved critical system issue",
                "Trained new team members",
                "Contributed to process improvement"
            };
            return achievements[_random.Next(achievements.Length)];
        }
        else
        {
            var achievements = new[]
            {
                "Completed assigned tasks",
                "Participated in team meetings",
                "Met basic job requirements",
                "Showed improvement in recent projects",
                "Contributed to team goals"
            };
            return achievements[_random.Next(achievements.Length)];
        }
    }
}
