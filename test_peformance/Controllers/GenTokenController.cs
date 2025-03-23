using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using test_peformance.Abstractions;

namespace test_peformance.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class GenTokenController : Controller
{
    private readonly IJwtTokenService _jwtTokenService;
     private readonly RedisCacheService _cacheService;

    public GenTokenController(IJwtTokenService jwtTokenService, RedisCacheService cacheService)
    {
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
    }

    [HttpPost]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        var email = request.Email;
        await _cacheService.SetAsync(email, request.Password);
        
        var claims = new List<Claim>
        {
            new("Id", "1"),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.NameIdentifier, email),
        };
        var token = _jwtTokenService.GenerateAccessToken(claims);
        return Ok(new
        {
            token = token,
        });
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

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }

}