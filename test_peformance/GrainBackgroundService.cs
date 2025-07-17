using test_peformance.Abstractions;
using test_peformance.Grains;

namespace test_peformance;

public class GrainBackgroundService : BackgroundService
{
    private readonly ILogger<GrainBackgroundService> _logger;
    
    private readonly IGrainFactory _grainFactory;

    public GrainBackgroundService(ILogger<GrainBackgroundService> logger, IGrainFactory grainFactory)
    {
        _logger = logger;
        _grainFactory = grainFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //First create the grain reference
        var friend1 = _grainFactory.GetGrain<IHelloGrain>("1");
        var friend2 = _grainFactory.GetGrain<IHelloGrain>("2");

        ChatGrain c = new ChatGrain();

        //Create a reference for chat, usable for subscribing to the observable grain.
        var obj = _grainFactory.CreateObjectReference<IChat>(c);

        //Subscribe the instance to receive messages.
        await friend1.Subscribe(obj);
        await friend2.Subscribe(obj);
    }
}