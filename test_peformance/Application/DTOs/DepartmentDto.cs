namespace test_peformance.Application.DTOs;

public class CreateDepartmentDto
{
    public string Name { get; set; }
}

public class UpdateDepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
