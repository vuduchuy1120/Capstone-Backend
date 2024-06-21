using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class WareHouse : EntityBase<int>
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string? Description { get; set; }
    public List<User>? Users { get; set; }
}
