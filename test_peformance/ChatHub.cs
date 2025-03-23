using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using test_peformance.Contants;

namespace test_peformance;

[Authorize(AuthenticationSchemes = AuthScheme.Hub)]
public class ChatHub: Hub
{
    public async Task SendMessage(string user, string message)
        => await Clients.All.SendAsync("ReceiveMessage", user, message);
}