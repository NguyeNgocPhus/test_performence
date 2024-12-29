using System.ComponentModel.DataAnnotations;

namespace test_peformance.Entities;

public class IntValueEntity
{
    [Key]
    public int Values { get; set; }
}