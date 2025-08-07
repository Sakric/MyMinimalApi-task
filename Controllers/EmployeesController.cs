using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmployeesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestEmployees()
    {
        var result = await _context.UserJobs
            .Include(uj => uj.User)
            .Include(uj => uj.Job)
            .OrderByDescending(uj => uj.DateTimeCreated)
            .Take(10)
            .ToListAsync();

        var response = result.Select(uj => new
        {
            Name = uj.User.Username.Split('_')[0],
            Last_name = uj.User.Lastname,
            Job = uj.Job.Name,
            time = uj.DateTimeCreated
        });

        return Ok(response);
    }

    [HttpGet("above-average")]
    public async Task<IActionResult> GetAboveAverageEmployees()
    {
        var averageJobCount = await _context.UserJobs
            .GroupBy(uj => uj.UserId)
            .Select(g => g.Count())
            .AverageAsync();

        var employees = await _context.UserJobs
            .Include(uj => uj.User)
            .GroupBy(uj => new { uj.UserId, uj.User.Username, uj.User.Lastname })
            .Select(g => new
            {
                UserId = g.Key.UserId,
                Username = g.Key.Username,
                Pavarde = g.Key.Lastname,
                DarbuKiekis = g.Count()
            })
            .Where(x => x.DarbuKiekis > averageJobCount)
            .OrderByDescending(x => x.DarbuKiekis)
            .Take(5)
            .ToListAsync();

        var result = employees.Select(x => new
        {
            Name = x.Username.Split('_')[0],
            Last_name = x.Pavarde,
            Jobs_count = x.DarbuKiekis,
        }).ToList();

        return Ok(result);
    }

    [HttpGet("most-active-in-30-days")]
    public async Task<IActionResult> GetMostActiveEmployees()
    {
        var now = DateTime.UtcNow;
        var before30Days = now.AddDays(-30);
        var before6Months = now.AddMonths(-6);

        var employees = await _context.UserJobs
            .Include(uj => uj.User)
            .Where(uj => uj.DateTimeCreated >= before6Months)
            .GroupBy(uj => new { uj.UserId, uj.User.Username, uj.User.Lastname })
            .Select(g => new
            {
                UserId = g.Key.UserId,
                Username = g.Key.Username,
                Last_name = g.Key.Lastname,
                WorkCount30Days = g.Count(x => x.DateTimeCreated >= before30Days),
                WorkCount6Months = g.Count(),
                SixMonthsCountAverage = g.Count() / 6.0
            })
            .Where(x => x.WorkCount30Days > x.SixMonthsCountAverage)
            .OrderByDescending(x => x.WorkCount30Days)
            .Take(5)
            .ToListAsync();

        var result = employees.Select(x => new
        {
            Name = x.Username.Split('_')[0],
            Last_name = x.Last_name,
            Jobs_done = x.WorkCount30Days
        }).ToList();

        return Ok(result);
    }
}