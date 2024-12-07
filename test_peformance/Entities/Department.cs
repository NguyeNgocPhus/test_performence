
using System.ComponentModel.DataAnnotations;

namespace test_peformance.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; }
    
    [ConcurrencyCheck]
    public Guid Version { get; set; }      
    public virtual ICollection<Employee> Employees { get; set; }
}