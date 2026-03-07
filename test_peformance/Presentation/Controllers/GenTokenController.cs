using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using test_peformance.Application.Abstractions;
using test_peformance.Application.DTOs;

namespace test_peformance.Presentation.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class GenTokenController : Controller
{
    private readonly IJwtTokenService _jwtTokenService;

    public GenTokenController(IJwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        var email = request.Email;
        var claims = new List<Claim>
        {
            new("Id", "1"),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.NameIdentifier, email),
        };
        var token = _jwtTokenService.GenerateAccessToken(claims);
        return Ok(new { token = token });
    }

    [HttpGet]
    public Task<ActionResult> GetMessages()
    {
        var claims = new List<Claim>
        {
            new("Id", "1"),
            new(ClaimTypes.Email, "admin@gmail.com"),
            new(ClaimTypes.NameIdentifier, "1"),
        };

        var token = _jwtTokenService.GenerateAccessToken(claims);
        return Task.FromResult<ActionResult>(Ok(token));
    }
}
