using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using test_peformance.Abstractions;
using test_peformance.Entities;

namespace test_peformance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CronjobController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CronjobController(ApplicationDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    [HttpGet]
    public async Task<ActionResult> StartJob()
    {
        var startTime = _dateTimeProvider.StartSync();
        var endTime = _dateTimeProvider.EndSync();
        var duration = _dateTimeProvider.Duration();
        // 👉 Công việc cần làm
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
            return Ok(false);
        }

        var endToday = DateTime.Today.Add(endTime);
        if (!isFirstRun && lastTime.TimeChanged >= endToday)
        {
            Log.Information("Not run cronjob because out of session end");
            return Ok(false);
        }

        var startToday = DateTime.Today.Add(startTime);
        var lastRun = lastTime?.TimeChanged ?? DateTime.MinValue;
        var isRunStartSync = lastRun < startToday && startTime <= nowTimeSpan;
        if (isRunStartSync)
        {
            RunJob("StartSync");
            Log.Information("Run cronjob in start of day");
            await SaveLastRun(lastTime);
            return Ok(true);
        }

        var isRunEndSync = lastRun.TimeOfDay < endTime && endTime <= nowTimeSpan;
        if (isRunEndSync)
        {
            RunJob("EndSync");
            Log.Information("Run cronjob in end of day");
            await SaveLastRun(lastTime);
            return Ok(true);
        }

        // ✅ 3. Chạy theo Duration kể từ StartSync
        var timeDiff = (nowTimeSpan - lastTime?.TimeChanged.TimeOfDay)?.Minutes ?? 0;
        if (timeDiff >= duration)
        {
            RunJob("Duration interval");
            Log.Information("Run cronjob in duration    ");
            await SaveLastRun(lastTime);
            return Ok(true);
        }

        Log.Information($"Not Run => Con lai {duration - timeDiff} minute");
        return Ok(false);
    }
    private void RunJob(string reason)
    {
        Log.Information("Job executed at {time}, reason = {reason}", DateTime.Now, reason);

        // 👉 Thực hiện logic xử lý của bạn ở đây
    }

    private async Task SaveLastRun(SystemConfig? systemConfig)
    {
        if (systemConfig == null) return;
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
            /*
             *Có một startTime dạng TimeSpan (ví dụ 1:00).

               Có một duration (ví dụ 30 phút).

               Sinh ra các mốc: 1:30, 2:00, 2:30, 3:00, …

               Cho một B (cũng là TimeSpan).

               Bạn muốn tìm mốc gần nhất nhưng không vượt quá B (tức là floor gần nhất).
             */
            var totalMinutes = (now.TimeOfDay - startTime).Minutes;
            var steps = (int)(totalMinutes / duration); // làm tròn xuống
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