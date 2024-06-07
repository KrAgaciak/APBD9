using APBD9.Data;
using APBD9.Models;
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

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] ClientTripDto clientTripDto)
    {
        // Check if the trip exists and is in the future
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrips && t.DateFrom > DateTime.Now);
        if (trip == null)
        {
            return NotFound("Trip does not exist or has already started.");
        }

        // Check if client by PESEL exists
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == clientTripDto.Pesel);
        if (client == null)
        {
            return NotFound("Client does not exist.");
        }

        // Check if this client is already assigned to this trip
        bool isAlreadyAssigned = await _context.ClientTrips.AnyAsync(ct => ct.IdTrip == idTrip && ct.IdClient == client.IdClient);
        if (isAlreadyAssigned)
        {
            return BadRequest("Client is already assigned to this trip.");
        }

        // Register the client to the trip
        var clientTrip = new ClientTrip
        {
            IdTrip = idTrip,
            IdClient = client.IdClient,
            RegisteredAt = DateTime.UtcNow, // Assuming RegisteredAt is the registration time
            PaymentDate = clientTripDto.PaymentDate
        };

        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync();

        return Ok("Client assigned to trip successfully.");
    }


}