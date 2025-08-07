using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("table")]
    public async Task<IActionResult> getUsers(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; 

        var query = _context.Users
            .Include(u => u.UserJobs)
            .ThenInclude(uj => uj.Job)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.ToLower();
            query = query.Where(u => 
                u.Username.ToLower().Contains(searchTerm) ||
                u.Lastname.ToLower().Contains(searchTerm) ||
                u.UserJobs.Any(uj => uj.Job.Name.ToLower().Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = result.Select(user => new
        {
            UserId = user.UserId,
            Name = user.Username.Split('_')[0],
            Last_name = user.Lastname,
            Jobs = user.UserJobs.Select(uj => new
            {
                JobId = uj.JobId,
                Job = uj.Job.Name,
                AssignedDate = uj.DateTimeCreated
            }).OrderByDescending(j => j.AssignedDate).ToList(),
            TotalJobs = user.UserJobs.Count(),
            LatestJobDate = user.UserJobs.Any() ? user.UserJobs.Max(uj => uj.DateTimeCreated) : (DateTime?)null
        });

        return Ok(new
        {
            data = response,
            pagination = new
            {
                currentPage = page,
                pageSize = pageSize,
                totalCount = totalCount,
                totalPages = totalPages,
                hasNextPage = page < totalPages,
                hasPreviousPage = page > 1
            }
        });
    }
}