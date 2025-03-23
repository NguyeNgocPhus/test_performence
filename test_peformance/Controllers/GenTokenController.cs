using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using test_peformance.Abstractions;

namespace test_peformance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenTokenController : Controller
{
    private readonly IJwtTokenService _jwtTokenService;

    public GenTokenController(IJwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    [HttpGet]
    public Task<ActionResult> GetMessages()
    {
        var claims = new List<Claim>
        {
            new("Id", "1"),
            new(ClaimTypes.Email, "admin@gmail.com"),
            new( ClaimTypes.NameIdentifier , "1"),
        };

        var token = _jwtTokenService.GenerateAccessToken(claims);
        return Task.FromResult<ActionResult>(Ok(token));
    }
}