public class Job
{
    public int JobId { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<UserJob> UserJobs { get; set; } = new List<UserJob>();
}