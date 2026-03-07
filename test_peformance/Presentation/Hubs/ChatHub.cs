using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using test_peformance.Infrastructure.Persistence.Constants;

namespace test_peformance.Presentation.Hubs;

[Authorize(AuthenticationSchemes = AuthScheme.Hub)]
public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
        => await Clients.All.SendAsync("ReceiveMessage", user, message);
}
