namespace test_peformance.Abstractions;

public interface ICustomLogger
{
    
    void LogInformation(string message, params object[] args);
    void LogError(Exception ex, string message, params object[] args);
    void LogWarning(string message, params object[] args);
}