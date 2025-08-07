using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserJob> UserJobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Allow dublicate Job names added UserJobId to s

        //modelBuilder.Entity<UserJob>()
        //    .HasKey(uj => new { uj.UserId, uj.JobId });


        modelBuilder.Entity<UserJob>()
            .HasOne(uj => uj.User)
            .WithMany(u => u.UserJobs)
            .HasForeignKey(uj => uj.UserId);

        modelBuilder.Entity<UserJob>()
            .HasOne(uj => uj.Job)
            .WithMany(j => j.UserJobs)
            .HasForeignKey(uj => uj.JobId);

        modelBuilder.Entity<User>()
           .HasIndex(u => u.Username)
           .IsUnique();
    }
}