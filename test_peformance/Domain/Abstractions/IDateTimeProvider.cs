namespace test_peformance.Domain.Abstractions;

public interface IDateTimeProvider
{
    public DateTime Now();
    public TimeSpan TimeOfDay();
    public TimeSpan StartSync();
    public TimeSpan EndSync();
    public int Duration();
}
