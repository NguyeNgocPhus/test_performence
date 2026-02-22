using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace test_peformance.Middleware;

public sealed class AddRywTokenAfterWriteFilter : IAsyncResultFilter
{
    private const string HeaderName = "X-RYW-Token";
    private readonly IRywTokenService _tokenService;
    private readonly int _pinSeconds;

    public AddRywTokenAfterWriteFilter(IRywTokenService tokenService, int pinSeconds = 5)
    {
        _tokenService = tokenService;
        _pinSeconds = pinSeconds;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var executed = await next();

        if (executed.HttpContext.Response.StatusCode is >= 200 and < 300)
        {
            var uid = executed.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? executed.HttpContext.User.FindFirstValue("sub");

            if (!string.IsNullOrEmpty(uid))
            {
                var exp = DateTimeOffset.UtcNow.AddSeconds(_pinSeconds);
                var token = _tokenService.Create(uid, exp);
                executed.HttpContext.Response.Headers[HeaderName] = token;
            }
        }
    }
}