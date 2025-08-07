public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Lastname { get; set; } = null!;

    public ICollection<UserJob> UserJobs { get; set; } = new List<UserJob>();
}