namespace test_peformance.Application.Cronjob;

public interface ICronjobService
{
    Task<bool> RunAsync();
}
