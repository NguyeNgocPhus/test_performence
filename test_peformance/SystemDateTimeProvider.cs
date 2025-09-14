using test_peformance.Abstractions;

namespace test_peformance;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime Now()
    {
        return DateTime.Now;
    }
    public TimeSpan TimeOfDay()
    {
        return DateTime.Now.TimeOfDay;
    } 

    public TimeSpan StartSync()
    {
        return DateTime.Now.TimeOfDay;
    }

    public TimeSpan EndSync()
    {
        return DateTime.Now.TimeOfDay;
    }

    public int Duration()
    {
        return 100;
    }
}