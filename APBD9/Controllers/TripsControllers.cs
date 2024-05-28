using APBD9.Data;
using Microsoft.AspNetCore.Mvc;

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
        
        
        return Ok();
    }

}