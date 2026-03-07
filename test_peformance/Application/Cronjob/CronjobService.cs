using Microsoft.EntityFrameworkCore;
using Serilog;
using test_peformance.Domain.Abstractions;
using test_peformance.Domain.Entities;
using test_peformance.Infrastructure.Persistence;

namespace test_peformance.Application.Cronjob;

public class CronjobService : ICronjobService
{
    private readonly ApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CronjobService(ApplicationDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> RunAsync()
    {
        var startTime = _dateTimeProvider.StartSync();
        var endTime = _dateTimeProvider.EndSync();
        var duration = _dateTimeProvider.Duration();
        var currentTime = _dateTimeProvider.Now();
        var nowTimeSpan = currentTime.TimeOfDay;
        var lastTime = await _context.SystemConfig.FirstOrDefaultAsync(x => x.Name == "Cronjob");
        var isFirstRun = false;
        if (lastTime == null)
        {
            isFirstRun = true;
            lastTime = await SaveFirstCronJob(startTime, duration);
        }

        var outOfSession = nowTimeSpan < startTime;
        if (outOfSession)
        {
            Log.Information("Not run cronjob because out of session start");
            return false;
        }

        var endToday = DateTime.Today.Add(endTime);
        if (!isFirstRun && lastTime.TimeChanged >= endToday)
        {
            Log.Information("Not run cronjob because out of session end");
            return false;
        }

        var startToday = DateTime.Today.Add(startTime);
        var lastRun = lastTime?.TimeChanged ?? DateTime.MinValue;
        var isRunStartSync = lastRun < startToday && startTime <= nowTimeSpan;
        if (isRunStartSync)
        {
            RunJob("StartSync");
            Log.Information("Run cronjob in start of day");
            await SaveLastRun(lastTime);
            return true;
        }

        var isRunEndSync = lastRun.TimeOfDay < endTime && endTime <= nowTimeSpan;
        if (isRunEndSync)
        {
            RunJob("EndSync");
            Log.Information("Run cronjob in end of day");
            await SaveLastRun(lastTime);
            return true;
        }

        var timeDiff = (nowTimeSpan - lastTime?.TimeChanged.TimeOfDay)?.Minutes ?? 0;
        if (timeDiff >= duration)
        {
            RunJob("Duration interval");
            Log.Information("Run cronjob in duration");
            await SaveLastRun(lastTime);
            return true;
        }

        Log.Information($"Not Run => Con lai {duration - timeDiff} minute");
        return false;
    }

    private void RunJob(string reason)
    {
        Log.Information("Job executed at {time}, reason = {reason}", DateTime.Now, reason);
    }

    private async Task SaveLastRun(SystemConfig systemConfig)
    {
        var now = _dateTimeProvider.Now();
        systemConfig.TimeChanged = now;
        _context.SystemConfig.Update(systemConfig);
        await _context.SaveChangesAsync();
    }

    private async Task<SystemConfig> SaveFirstCronJob(TimeSpan startTime, int duration)
    {
        var now = _dateTimeProvider.Now();

        var lastrun = new SystemConfig
        {
            Name = "Cronjob",
            Value = "1",
        };
        if (now.TimeOfDay < startTime)
            lastrun.TimeChanged = DateTime.MinValue;
        else
        {
            var totalMinutes = (now.TimeOfDay - startTime).Minutes;
            var steps = (int)(totalMinutes / duration);
            if (totalMinutes % duration == 0)
                steps -= 1;
            var nearest = startTime.Add(TimeSpan.FromMinutes(steps * duration));
            lastrun.TimeChanged = DateTime.Today.Add(nearest);
        }

        _context.SystemConfig.Add(lastrun);
        await _context.SaveChangesAsync();
        return lastrun;
    }
}
