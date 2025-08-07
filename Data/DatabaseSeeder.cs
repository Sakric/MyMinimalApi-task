using Microsoft.EntityFrameworkCore;
using System.Text;

public static class DatabaseSeeder
{
    private static readonly Random _random = new Random();
    
    private static readonly string[] _jobNames = {
        "Cleaned the office floors", "Updated the website software", "Prepared lunch for the team", "Fixed the broken printer",
        "Organized the file cabinet", "Debugged the login system", "Washed the company car", "Installed security patches",
        "Watered the office plants", "Backed up the database", "Cleaned the coffee machine", "Tested the mobile app",
        "Painted the conference room", "Optimized the server performance", "Prepared the monthly report", "Fixed the air conditioning",
        "Sorted the mail delivery", "Updated customer records", "Cleaned the windows", "Configured the new router",
        "Prepared sandwiches for lunch", "Reset user passwords", "Organized the supply closet", "Monitored network traffic",
        "Cleaned the kitchen area", "Updated the inventory system", "Prepared the presentation slides", "Fixed the door lock",
        "Arranged the meeting room", "Deployed the latest code", "Prepared coffee for everyone", "Troubleshot WiFi issues",
        "Cleaned the parking lot", "Updated social media posts", "Prepared the budget forecast", "Repaired the office chair",
        "Organized team documents", "Tested the payment system", "Prepared the training materials", "Fixed the bathroom sink",
        "Cleaned the computer screens", "Updated the contact database", "Prepared the project timeline", "Installed new software",
        "Organized the bookshelf", "Monitored system alerts", "Prepared the quarterly review", "Fixed the projector",
        "Cleaned the reception area", "Updated the user manual", "Prepared the team schedule", "Configured email settings"
    };
    
    private static readonly string[] _firstNames = {
        "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda",
        "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica",
        "Thomas", "Sarah", "Charles", "Karen", "Christopher", "Nancy", "Daniel", "Lisa",
        "Matthew", "Betty", "Anthony", "Helen", "Mark", "Sandra", "Donald", "Donna",
        "Steven", "Carol", "Paul", "Ruth", "Andrew", "Sharon", "Joshua", "Michelle",
        "Kenneth", "Laura", "Kevin", "Sarah", "Brian", "Kimberly", "George", "Deborah",
        "Timothy", "Dorothy", "Ronald", "Lisa", "Edward", "Nancy", "Jason", "Karen",
        "Jeffrey", "Betty", "Ryan", "Helen", "Jacob", "Sandra", "Gary", "Donna",
        "Nicholas", "Carol", "Eric", "Ruth", "Jonathan", "Sharon", "Stephen", "Michelle",
        "Larry", "Laura", "Justin", "Sarah", "Scott", "Kimberly", "Brandon", "Deborah",
        "Benjamin", "Dorothy", "Samuel", "Lisa", "Gregory", "Nancy", "Frank", "Karen",
        "Raymond", "Betty", "Alexander", "Helen", "Patrick", "Sandra", "Jack", "Donna",
        "Dennis", "Carol", "Jerry", "Ruth", "Tyler", "Sharon", "Aaron", "Michelle"
    };
    
    private static readonly string[] _lastNames = {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas",
        "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White",
        "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker", "Young",
        "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
        "Green", "AdAMS", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell",
        "Carter", "Roberts", "Gomez", "Phillips", "EvANS", "Turner", "Diaz", "Parker",
        "Cruz", "Edwards", "Collins", "Reyes", "Stewart", "Morris", "Morales", "Murphy",
        "Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper", "Peterson", "Bailey",
        "Reed", "Kelly", "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson",
        "Watson", "Brooks", "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza",
        "Ruiz", "Hughes", "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers"
    };

    public static async Task SeedDatabaseAsync(ApplicationDbContext context, int totalRecords = 1_000_000)
    {
        if (context.Jobs.Any())
        {
            Console.WriteLine("Database already contains data. Skipping seeding.");
            return;
        }

        Console.WriteLine($"Starting database seeding with {totalRecords:N0} total records...");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        int jobCount = Math.Min(500, _jobNames.Length); // max 500
        int userCount = Math.Min(100_000, totalRecords / 10); // max 100,000 users
        int userJobCount = totalRecords - jobCount - userCount; // rest

        Console.WriteLine($"Creating {jobCount:N0} jobs, {userCount:N0} users, {userJobCount:N0} user-job relationships...");

        await SeedJobsAsync(context, jobCount);
        
        await SeedUsersAsync(context, userCount);
        
        await SeedUserJobsAsync(context, userJobCount, jobCount, userCount);

        stopwatch.Stop();
        Console.WriteLine($"Database seeding completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
    }

    private static async Task SeedJobsAsync(ApplicationDbContext context, int count)
    {
        Console.WriteLine("Seeding jobs...");
        
        var jobs = new List<Job>(count);
        
        for (int i = 0; i < count; i++)
        {
            string jobName = _jobNames[_random.Next(_jobNames.Length)];
            
            jobs.Add(new Job { Name = jobName });
        }
        
        await context.Jobs.AddRangeAsync(jobs);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ {count:N0} jobs created");
    }

    private static async Task SeedUsersAsync(ApplicationDbContext context, int count)
    {
        Console.WriteLine("Seeding users...");
        
        const int batchSize = 10_000;
        var totalBatches = (count + batchSize - 1) / batchSize;
        int userCounter = 1;

        for (int batch = 0; batch < totalBatches; batch++)
        {
            var currentBatchSize = Math.Min(batchSize, count - (batch * batchSize));
            var users = new List<User>(currentBatchSize);
            
            for (int i = 0; i < currentBatchSize; i++)
            {
                var firstName = _firstNames[_random.Next(_firstNames.Length)];
                var lastName = _lastNames[_random.Next(_lastNames.Length)];
                
                var uniqueId = _random.Next(1, 999999);
                
                users.Add(new User 
                { 
                    Username = $"{firstName}_{lastName}_{userCounter}",
                    Lastname = lastName
                });

                userCounter++;
            }

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            
            Console.WriteLine($"  Progress: {Math.Min((batch + 1) * batchSize, count):N0}/{count:N0} users");
        }
        
        Console.WriteLine($"✓ {count:N0} users created");
    }

    private static async Task SeedUserJobsAsync(ApplicationDbContext context, int count, int maxJobId, int maxUserId)
    {
        Console.WriteLine("Seeding user-job relationships...");
        
        const int batchSize = 50_000;
        var totalBatches = (count + batchSize - 1) / batchSize;
        var baseDate = DateTime.UtcNow.AddYears(-2);
        
        for (int batch = 0; batch < totalBatches; batch++)
        {
            var currentBatchSize = Math.Min(batchSize, count - (batch * batchSize));
            var userJobs = new List<UserJob>(currentBatchSize);
            
            for (int i = 0; i < currentBatchSize; i++)
            {
                var userId = _random.Next(1, maxUserId + 1);
                var jobId = _random.Next(1, maxJobId + 1);
                
                var randomDate = baseDate.AddDays(_random.Next(0, 730))
                                        .AddHours(_random.Next(0, 24))
                                        .AddMinutes(_random.Next(0, 60))
                                        .AddSeconds(_random.Next(0, 60));
                
                userJobs.Add(new UserJob
                {
                    UserId = userId,
                    JobId = jobId,
                    DateTimeCreated = randomDate
                });
            }
            
            await context.UserJobs.AddRangeAsync(userJobs);
            await context.SaveChangesAsync();
            
            Console.WriteLine($"  Progress: {Math.Min((batch + 1) * batchSize, count):N0}/{count:N0} relationships");
        }
        
        Console.WriteLine($"✓ {count:N0} user-job relationships created");
    }
}