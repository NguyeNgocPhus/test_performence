using System.Security.Claims;

namespace test_peformance.Middleware;

public sealed class RywDecision
{
    public DbRole Role { get; set; } = DbRole.Replica;
    public bool IsPinned { get; set; }
    public int? PinRemainingMs { get; set; }
}
public enum DbRole { Master, Replica }

public sealed class RywRoutingMiddleware : IMiddleware
{
    private const string HeaderName = "X-RYW-Token";

    private readonly IRywTokenService _tokenService;
    private readonly RywDecision _decision;

    public RywRoutingMiddleware(IRywTokenService tokenService, RywDecision decision)
    {
        _tokenService = tokenService;
        _decision = decision;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _decision.Role = DbRole.Replica;

        var uid = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? context.User.FindFirstValue("sub");

        if (!string.IsNullOrEmpty(uid) &&
            context.Request.Headers.TryGetValue(HeaderName, out var tokenValues))
        {
            var token = tokenValues.ToString();
            if (_tokenService.TryValidate(token, uid, DateTimeOffset.UtcNow, out var exp))
            {
                _decision.Role = DbRole.Master;
                _decision.IsPinned = true;
                _decision.PinRemainingMs = (int)Math.Max(0, (exp - DateTimeOffset.UtcNow).TotalMilliseconds);
            }
        }

        await next(context);
    }
}