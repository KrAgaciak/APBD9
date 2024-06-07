using APBD9.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD9.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsControllers: ControllerBase
{
    private readonly ApbdContext _context;

    public TripsControllers(ApbdContext _context)
    {
        this._context = _context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips()
    {
        // Implementation of fetching trips without sorting and pagination
        var trips = await _context.Trips.ToListAsync();
        return Ok(trips);
    }

    // New GET method with sorting and pagination
    [HttpGet("trips")]
    public async Task<IActionResult> GetTripsSortedAndPaged(int? page, int pageSize = 10)
    {
        // Calculate the number of items to skip
        int pageNumber = page ?? 1;
        int skipAmount = (pageNumber - 1) * pageSize;

        // Fetch trips sorted by start date in descending order with pagination
        var trips = await _context.Trips
            .OrderByDescending(t => t.DateFrom)
            .Skip(skipAmount)
            .Take(pageSize)
            .ToListAsync();

        return Ok(trips);
    }

}