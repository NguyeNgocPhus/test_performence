using System.ComponentModel.DataAnnotations;

namespace test_peformance.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; }

    [ConcurrencyCheck]
    public Guid Version { get; set; }
    public virtual ICollection<Employee> Employees { get; set; }
}
