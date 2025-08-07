public class UserJob
{
    // added primary key to have dublicate jobs
    public int UserJobId { get; set; }
    public int UserId { get; set; }
    public int JobId { get; set; }
    public DateTime DateTimeCreated { get; set; }

    public User User { get; set; } = null!;
    public Job Job { get; set; } = null!;
}