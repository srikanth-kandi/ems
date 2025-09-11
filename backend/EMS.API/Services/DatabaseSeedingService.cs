using EMS.API.Data;
using EMS.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace EMS.API.Services;

public class DatabaseSeedingService
{
    private readonly EMSDbContext _context;
    private readonly ILogger<DatabaseSeedingService> _logger;
    private readonly Random _random = new Random();

    // Sample data arrays
    private readonly string[] _firstNames = {
        "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth",
        "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Christopher", "Karen",
        "Charles", "Nancy", "Daniel", "Lisa", "Matthew", "Betty", "Anthony", "Helen", "Mark", "Sandra",
        "Donald", "Donna", "Steven", "Carol", "Paul", "Ruth", "Andrew", "Sharon", "Joshua", "Michelle",
        "Kenneth", "Laura", "Kevin", "Sarah", "Brian", "Kimberly", "George", "Deborah", "Edward", "Dorothy",
        "Ronald", "Lisa", "Timothy", "Nancy", "Jason", "Karen", "Jeffrey", "Betty", "Ryan", "Helen",
        "Jacob", "Sandra", "Gary", "Donna", "Nicholas", "Carol", "Eric", "Ruth", "Jonathan", "Sharon",
        "Stephen", "Michelle", "Larry", "Laura", "Justin", "Sarah", "Scott", "Kimberly", "Brandon", "Deborah",
        "Benjamin", "Dorothy", "Samuel", "Amy", "Gregory", "Angela", "Alexander", "Ashley", "Patrick", "Brenda",
        "Jack", "Emma", "Dennis", "Olivia", "Jerry", "Cynthia", "Tyler", "Marie", "Aaron", "Janet",
        "Jose", "Catherine", "Henry", "Frances", "Adam", "Christine", "Douglas", "Samantha", "Nathan", "Debra",
        "Peter", "Rachel", "Zachary", "Carolyn", "Kyle", "Janet", "Noah", "Virginia", "Alan", "Maria",
        "Ethan", "Heather", "Jeremy", "Diane", "Mason", "Julie", "Christian", "Joyce", "Keith", "Victoria",
        "Roger", "Kelly", "Terry", "Christina", "Gerald", "Joan", "Harold", "Evelyn", "Sean", "Judith",
        "Austin", "Megan", "Carl", "Cheryl", "Arthur", "Andrea", "Lawrence", "Hannah", "Dylan", "Jacqueline",
        "Jesse", "Martha", "Jordan", "Gloria", "Bryan", "Teresa", "Billy", "Sara", "Joe", "Janice",
        "Bruce", "Julia", "Gabriel", "Marie", "Wayne", "Madison", "Roy", "Grace", "Ralph", "Judy",
        "Eugene", "Theresa", "Louis", "Beverly", "Philip", "Denise", "Bobby", "Marilyn", "Johnny", "Amber",
        "Willie", "Danielle", "Larry", "Rose", "Jimmy", "Brittany", "Albert", "Diana", "Wayne", "Abigail",
        "Eugene", "Jane", "Ralph", "Lori", "Philip", "Bobby", "Johnny", "Willie", "Jimmy", "Albert"
    };

    private readonly string[] _lastNames = {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
        "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
        "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
        "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
        "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
        "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker", "Cruz", "Edwards", "Collins", "Reyes",
        "Stewart", "Morris", "Morales", "Murphy", "Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper",
        "Peterson", "Bailey", "Reed", "Kelly", "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson",
        "Watson", "Brooks", "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza", "Ruiz", "Hughes",
        "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers", "Long", "Ross", "Foster", "Jimenez",
        "Powell", "Jenkins", "Perry", "Russell", "Sullivan", "Bell", "Coleman", "Butler", "Henderson", "Barnes",
        "Gonzales", "Fisher", "Vasquez", "Simmons", "Romero", "Jordan", "Patterson", "Alexander", "Hamilton", "Graham",
        "Reynolds", "Griffin", "Wallace", "Moreno", "West", "Cole", "Hayes", "Bryant", "Herrera", "Gibson",
        "Ellis", "Tran", "Medina", "Aguilar", "Stevens", "Murray", "Ford", "Castro", "Marshall", "Owens",
        "Harrison", "Fernandez", "McDonald", "Woods", "Washington", "Kennedy", "Wells", "Vargas", "Henry", "Chen",
        "Freeman", "Webb", "Tucker", "Guzman", "Burns", "Crawford", "Olson", "Simpson", "Porter", "Hunter",
        "Gordon", "Mendez", "Silva", "Shaw", "Snyder", "Mason", "Dixon", "Munoz", "Hunt", "Hicks",
        "Holmes", "Palmer", "Wagner", "Black", "Robertson", "Boyd", "Rose", "Stone", "Salazar", "Fox",
        "Warren", "Mills", "Meyer", "Rice", "Schmidt", "Garza", "Daniels", "Ferguson", "Nichols", "Stephens",
        "Soto", "Wells", "Weaver", "Ryan", "Larson", "Frazier", "Dunn", "Bowman", "May", "Holland",
        "Terry", "Carpenter", "Bishop", "Lane", "Andrews", "Riley", "Johnston", "Elliott", "Hansen", "Ray",
        "Arnold", "Woods", "Spencer", "Pierce", "Grant", "Bates", "Watts", "Hale", "Baldwin", "Norris"
    };

    private readonly string[] _positions = {
        "Software Engineer", "Senior Software Engineer", "Lead Developer", "Technical Lead", "Software Architect",
        "Full Stack Developer", "Frontend Developer", "Backend Developer", "DevOps Engineer", "Cloud Engineer",
        "Data Scientist", "Data Analyst", "Business Analyst", "Product Manager", "Project Manager",
        "Scrum Master", "QA Engineer", "Test Automation Engineer", "UI/UX Designer", "Graphic Designer",
        "Marketing Manager", "Digital Marketing Specialist", "Content Writer", "Social Media Manager",
        "Sales Representative", "Sales Manager", "Account Manager", "Customer Success Manager",
        "HR Manager", "HR Specialist", "Recruiter", "Training Coordinator", "Compensation Analyst",
        "Financial Analyst", "Accountant", "Senior Accountant", "Financial Controller", "CFO",
        "Operations Manager", "Operations Analyst", "Supply Chain Manager", "Procurement Specialist",
        "IT Support Specialist", "System Administrator", "Network Administrator", "Database Administrator",
        "Security Analyst", "Compliance Officer", "Legal Counsel", "Paralegal",
        "Executive Assistant", "Administrative Assistant", "Office Manager", "Facilities Manager",
        "Research Scientist", "Research Analyst", "Lab Technician", "Quality Assurance Manager",
        "Customer Service Representative", "Customer Support Manager", "Technical Writer", "Documentation Specialist"
    };

    private readonly string[] _departments = {
        "Engineering", "Product Management", "Marketing", "Sales", "Human Resources", "Finance", "Operations",
        "Customer Success", "IT Support", "Legal", "Research & Development", "Quality Assurance", "Design",
        "Data & Analytics", "Security", "Compliance", "Facilities", "Administration", "Business Development",
        "Strategic Planning", "Corporate Communications", "Investor Relations", "Procurement", "Supply Chain"
    };

    private readonly string[] _addresses = {
        "123 Main St, New York, NY 10001", "456 Oak Ave, Los Angeles, CA 90210", "789 Pine Rd, Chicago, IL 60601",
        "321 Elm St, Houston, TX 77001", "654 Maple Dr, Phoenix, AZ 85001", "987 Cedar Ln, Philadelphia, PA 19101",
        "147 Birch St, San Antonio, TX 78201", "258 Spruce Ave, San Diego, CA 92101", "369 Willow Rd, Dallas, TX 75201",
        "741 Poplar St, San Jose, CA 95101", "852 Ash Ave, Austin, TX 78701", "963 Hickory Dr, Jacksonville, FL 32201",
        "159 Cherry Ln, Fort Worth, TX 76101", "357 Walnut St, Columbus, OH 43201", "468 Chestnut Ave, Charlotte, NC 28201",
        "579 Sycamore Rd, San Francisco, CA 94101", "680 Dogwood St, Indianapolis, IN 46201", "791 Magnolia Ave, Seattle, WA 98101",
        "802 Redwood Dr, Denver, CO 80201", "913 Fir Ln, Washington, DC 20001", "024 Hemlock St, Boston, MA 02101",
        "135 Cypress Ave, El Paso, TX 79901", "246 Juniper Rd, Nashville, TN 37201", "357 Sequoia St, Detroit, MI 48201",
        "468 Pinecone Ave, Oklahoma City, OK 73101", "579 Acorn Dr, Portland, OR 97201", "680 Conifer Ln, Las Vegas, NV 89101",
        "791 Spruce St, Louisville, KY 40201", "802 Cedar Ave, Baltimore, MD 21201", "913 Oak Rd, Milwaukee, WI 53201"
    };

    private readonly string[] _phoneNumbers = {
        "(555) 123-4567", "(555) 234-5678", "(555) 345-6789", "(555) 456-7890", "(555) 567-8901",
        "(555) 678-9012", "(555) 789-0123", "(555) 890-1234", "(555) 901-2345", "(555) 012-3456",
        "(555) 111-2222", "(555) 222-3333", "(555) 333-4444", "(555) 444-5555", "(555) 555-6666",
        "(555) 666-7777", "(555) 777-8888", "(555) 888-9999", "(555) 999-0000", "(555) 000-1111",
        "(555) 101-2020", "(555) 202-3030", "(555) 303-4040", "(555) 404-5050", "(555) 505-6060",
        "(555) 606-7070", "(555) 707-8080", "(555) 808-9090", "(555) 909-0101", "(555) 010-2020"
    };

    private readonly string[] _performanceComments = {
        "Excellent performance throughout the quarter. Consistently exceeds expectations.",
        "Strong contributor to team goals. Shows great initiative and leadership.",
        "Meets all performance standards. Reliable and dependable team member.",
        "Shows potential for growth. Needs more experience in current role.",
        "Outstanding work ethic and dedication. Goes above and beyond regularly.",
        "Good team player with solid technical skills. Room for improvement in communication.",
        "Consistently delivers quality work on time. Strong problem-solving abilities.",
        "Shows initiative in taking on new challenges. Good potential for advancement.",
        "Reliable performer who meets expectations. Could benefit from additional training.",
        "Exceptional performance in all areas. Natural leader and mentor to others.",
        "Solid contributor with good technical skills. Working on improving soft skills.",
        "Meets expectations with occasional excellence. Steady and consistent performer.",
        "Shows strong analytical thinking. Good at identifying process improvements.",
        "Excellent communication skills. Great at collaborating with cross-functional teams.",
        "Strong work ethic and attention to detail. Consistently produces high-quality work.",
        "Good technical skills with room for growth. Shows enthusiasm for learning.",
        "Reliable team member who supports others. Consistently meets deadlines.",
        "Shows potential for leadership roles. Good at mentoring junior team members.",
        "Strong problem-solving abilities. Good at working independently and in teams.",
        "Consistently exceeds performance targets. Great at managing multiple priorities."
    };

    private readonly string[] _goals = {
        "Improve technical skills in cloud technologies", "Enhance leadership and mentoring abilities",
        "Complete advanced certification in relevant field", "Increase productivity by 15%",
        "Develop better communication skills", "Lead a major project from start to finish",
        "Improve customer satisfaction scores", "Learn new programming languages",
        "Enhance data analysis capabilities", "Improve time management skills",
        "Develop expertise in agile methodologies", "Increase team collaboration",
        "Complete professional development courses", "Improve presentation skills",
        "Enhance problem-solving techniques", "Develop strategic thinking abilities",
        "Improve cross-functional collaboration", "Learn new industry best practices",
        "Enhance project management skills", "Develop expertise in emerging technologies"
    };

    private readonly string[] _achievements = {
        "Successfully completed major project ahead of schedule", "Improved team efficiency by 20%",
        "Received recognition for outstanding customer service", "Led successful product launch",
        "Implemented cost-saving measures worth $50K", "Mentored 3 junior team members",
        "Completed advanced certification with distinction", "Reduced process time by 30%",
        "Received employee of the month award", "Successfully managed high-priority client account",
        "Developed innovative solution that saved company resources", "Led cross-functional team initiative",
        "Improved system performance by 40%", "Successfully resolved critical production issue",
        "Completed professional development program", "Received positive feedback from stakeholders",
        "Implemented new process that increased accuracy", "Successfully managed budget under target",
        "Led successful team building initiative", "Received recognition for innovation"
    };

    public DatabaseSeedingService(EMSDbContext context, ILogger<DatabaseSeedingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedDatabaseAsync()
    {
        _logger.LogInformation("Starting database seeding process...");

        try
        {
            // Check if data already exists
            if (await _context.Departments.AnyAsync())
            {
                _logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            await SeedDepartmentsAsync();
            await SeedUsersAsync();
            await SeedEmployeesAsync();
            await SeedAttendanceAsync();
            await SeedPerformanceMetricsAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding.");
            throw;
        }
    }

    public async Task ReseedDatabaseAsync()
    {
        _logger.LogInformation("Starting database reseeding process...");

        try
        {
            // Clear existing data
            _context.PerformanceMetrics.RemoveRange(_context.PerformanceMetrics);
            _context.Attendances.RemoveRange(_context.Attendances);
            _context.Employees.RemoveRange(_context.Employees);
            _context.Users.RemoveRange(_context.Users);
            _context.Departments.RemoveRange(_context.Departments);

            await _context.SaveChangesAsync();

            // Seed fresh data
            await SeedDepartmentsAsync();
            await SeedUsersAsync();
            await SeedEmployeesAsync();
            await SeedAttendanceAsync();
            await SeedPerformanceMetricsAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Database reseeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database reseeding.");
            throw;
        }
    }

    private async Task SeedDepartmentsAsync()
    {
        _logger.LogInformation("Seeding departments...");

        var departments = new List<Department>();
        var baseDate = DateTime.UtcNow.AddYears(-2);

        for (int i = 0; i < _departments.Length; i++)
        {
            departments.Add(new Department
            {
                Id = i + 1,
                Name = _departments[i],
                Description = $"Description for {_departments[i]} Department",
                ManagerName = $"{_firstNames[_random.Next(_firstNames.Length)]} {_lastNames[_random.Next(_lastNames.Length)]}",
                CreatedAt = baseDate.AddDays(_random.Next(365)),
                UpdatedAt = _random.Next(2) == 0 ? baseDate.AddDays(_random.Next(365, 730)) : null
            });
        }

        _context.Departments.AddRange(departments);
        await _context.SaveChangesAsync();
    }

    private async Task SeedUsersAsync()
    {
        _logger.LogInformation("Seeding users...");

        var users = new List<User>
        {
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@ems.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddYears(-2),
                LastLoginAt = DateTime.UtcNow.AddDays(-1)
            },
            new User
            {
                Id = 2,
                Username = "hr_manager",
                Email = "hr@ems.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("hr123"),
                Role = "HR",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddYears(-1),
                LastLoginAt = DateTime.UtcNow.AddDays(-3)
            },
            new User
            {
                Id = 3,
                Username = "manager",
                Email = "manager@ems.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                Role = "Manager",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-6),
                LastLoginAt = DateTime.UtcNow.AddDays(-7)
            }
        };

        // Add additional users
        for (int i = 4; i <= 50; i++)
        {
            var firstName = _firstNames[_random.Next(_firstNames.Length)];
            var lastName = _lastNames[_random.Next(_lastNames.Length)];
            var username = $"{firstName.ToLower()}.{lastName.ToLower()}";
            var email = $"{username}@ems.com";

            users.Add(new User
            {
                Id = i,
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = _random.Next(3) switch
                {
                    0 => "Employee",
                    1 => "Manager",
                    _ => "HR"
                },
                IsActive = _random.Next(10) != 0, // 90% active
                CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(730)),
                LastLoginAt = _random.Next(2) == 0 ? DateTime.UtcNow.AddDays(-_random.Next(30)) : null
            });
        }

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
    }

    private async Task SeedEmployeesAsync()
    {
        _logger.LogInformation("Seeding employees...");

        var employees = new List<Employee>();
        var baseDate = DateTime.UtcNow.AddYears(-3);

        for (int i = 1; i <= 500; i++)
        {
            var firstName = _firstNames[_random.Next(_firstNames.Length)];
            var lastName = _lastNames[_random.Next(_lastNames.Length)];
            var email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@ems.com";
            var dateOfBirth = DateTime.UtcNow.AddYears(-_random.Next(22, 65));
            var dateOfJoining = baseDate.AddDays(_random.Next(1095)); // Within last 3 years
            var salary = _random.Next(40000, 150000);
            var departmentId = _random.Next(1, _departments.Length + 1);

            employees.Add(new Employee
            {
                Id = i,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = _phoneNumbers[_random.Next(_phoneNumbers.Length)],
                Address = _addresses[_random.Next(_addresses.Length)],
                DateOfBirth = dateOfBirth,
                DateOfJoining = dateOfJoining,
                Position = _positions[_random.Next(_positions.Length)],
                Salary = salary,
                DepartmentId = departmentId,
                IsActive = _random.Next(10) != 0, // 90% active
                CreatedAt = dateOfJoining,
                UpdatedAt = _random.Next(3) == 0 ? dateOfJoining.AddDays(_random.Next(1, 365)) : null
            });
        }

        _context.Employees.AddRange(employees);
        await _context.SaveChangesAsync();
    }

    private async Task SeedAttendanceAsync()
    {
        _logger.LogInformation("Seeding attendance records...");

        var attendances = new List<Attendance>();
        var startDate = DateTime.Today.AddDays(-365); // Last year
        var activeEmployees = await _context.Employees.Where(e => e.IsActive).ToListAsync();

        foreach (var employee in activeEmployees)
        {
            var currentDate = startDate;
            while (currentDate <= DateTime.Today)
            {
                // Skip weekends for most employees (80% chance)
                if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (_random.Next(10) < 8) // 80% skip weekends
                    {
                        currentDate = currentDate.AddDays(1);
                        continue;
                    }
                }

                // Skip some random days (sick days, vacation, etc.)
                if (_random.Next(20) == 0) // 5% chance of absence
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                var checkInTime = currentDate.AddHours(8).AddMinutes(_random.Next(-30, 60)); // 7:30 AM to 9:00 AM
                var checkOutTime = checkInTime.AddHours(8).AddMinutes(_random.Next(-60, 120)); // 7-10 hours later
                var totalHours = checkOutTime - checkInTime;

                attendances.Add(new Attendance
                {
                    EmployeeId = employee.Id,
                    CheckInTime = checkInTime,
                    CheckOutTime = checkOutTime,
                    TotalHours = totalHours,
                    Date = currentDate,
                    Notes = _random.Next(10) == 0 ? "Overtime work" : null,
                    CreatedAt = checkInTime,
                    UpdatedAt = _random.Next(5) == 0 ? checkOutTime.AddMinutes(_random.Next(1, 60)) : null
                });

                currentDate = currentDate.AddDays(1);
            }
        }

        _context.Attendances.AddRange(attendances);
        await _context.SaveChangesAsync();
    }

    private async Task SeedPerformanceMetricsAsync()
    {
        _logger.LogInformation("Seeding performance metrics...");

        var performanceMetrics = new List<PerformanceMetric>();
        var activeEmployees = await _context.Employees.Where(e => e.IsActive).ToListAsync();
        var currentYear = DateTime.Now.Year;

        foreach (var employee in activeEmployees)
        {
            // Generate performance metrics for the last 2 years
            for (int year = currentYear - 1; year <= currentYear; year++)
            {
                for (int quarter = 1; quarter <= 4; quarter++)
                {
                    var performanceScore = _random.Next(60, 101); // 60-100 range
                    var comments = _performanceComments[_random.Next(_performanceComments.Length)];
                    var goals = _goals[_random.Next(_goals.Length)];
                    var achievements = _achievements[_random.Next(_achievements.Length)];

                    performanceMetrics.Add(new PerformanceMetric
                    {
                        EmployeeId = employee.Id,
                        Year = year,
                        Quarter = quarter,
                        PerformanceScore = performanceScore,
                        Comments = comments,
                        Goals = goals,
                        Achievements = achievements,
                        CreatedAt = new DateTime(year, quarter * 3, 1).AddDays(_random.Next(30)),
                        UpdatedAt = _random.Next(3) == 0 ? new DateTime(year, quarter * 3, 1).AddDays(_random.Next(30, 60)) : null
                    });
                }
            }
        }

        _context.PerformanceMetrics.AddRange(performanceMetrics);
        await _context.SaveChangesAsync();
    }
}
