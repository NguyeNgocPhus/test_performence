namespace test_peformance.Events;

public class DepartmentMessage : QueueMessage
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
}