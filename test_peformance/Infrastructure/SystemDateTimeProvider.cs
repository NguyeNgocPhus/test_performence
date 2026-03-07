using test_peformance.Domain.Abstractions;

namespace test_peformance.Infrastructure;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime Now() => DateTime.Now;

    public TimeSpan TimeOfDay() => DateTime.Now.TimeOfDay;

    public TimeSpan StartSync() => DateTime.Now.TimeOfDay;

    public TimeSpan EndSync() => DateTime.Now.TimeOfDay;

    public int Duration() => 100;
}
