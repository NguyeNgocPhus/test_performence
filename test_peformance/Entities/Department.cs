namespace test_peformance.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; }
    public ICollection<Employee> Employees { get; set; }
    public override string ToString()
    {
        return "ok";
    }
}