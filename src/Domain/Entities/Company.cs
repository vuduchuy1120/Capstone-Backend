using Contract.Services.Company.Create;
using Contract.Services.Company.Shared;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Company : EntityBase<Guid>
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string DirectorName { get; set; }
    public string DirectorPhone { get; set; }
    public string Email { get; set; }
    public CompanyType CompanyType {  get; set; }
    public List<Order>? Order { get; set; }
    public List<User>? Users { get; set; }
    public List<ProductPhase>? ProductPhases { get; set; }

    public static Company Create(CreateCompanyCommand request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Address = request.Address,
            DirectorName = request.DirectorName,
            DirectorPhone = request.DirectorPhone,
            Email = request.Email,
            CompanyType = request.CompanyType,
            Name = request.Name,
        };
    }
}
