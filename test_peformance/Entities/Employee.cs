namespace test_peformance.Entities;

public class Employee : BaseEntity
{
    public string? Name { get; set; }
    public decimal Salary { get; set; }
    public Department? Department { get; set; }
    public int DepartmentId { get; set; }
}