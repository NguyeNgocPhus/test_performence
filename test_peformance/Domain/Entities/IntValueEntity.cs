using System.ComponentModel.DataAnnotations;

namespace test_peformance.Domain.Entities;

public class IntValueEntity
{
    [Key]
    public int Values { get; set; }
}
