using Contract.Abstractions.Shared.Utils;
using Contract.Services.Company.Create;
using Contract.Services.Company.Updates;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Company : EntityBase<Guid>
{
    public string Name { get; set; }
    public string? NameUnAccent { get; set; }
    public string Address { get; set; }
    public string? AddressUnAccent { get; set; }
    public string DirectorName { get; set; }
    public string? DirectorNameUnAccent { get; set; }
    public string DirectorPhone { get; set; }
    public string? Email { get; set; }
    public string CompanyType { get; set; }
    public string CompanyTypeUnAccent { get; set; }
    public List<Order>? Orders { get; set; }
    public List<User>? Users { get; set; }

    public static Company Create(CreateCompanyRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Address = request.CompanyRequest.Address,
            AddressUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.Address),
            DirectorName = request.CompanyRequest.DirectorName,
            DirectorNameUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.DirectorName),
            DirectorPhone = request.CompanyRequest.DirectorPhone,
            Email = request.CompanyRequest.Email,
            CompanyType = request.CompanyRequest.CompanyType,
            CompanyTypeUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.CompanyType),
            Name = request.CompanyRequest.Name,
            NameUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.Name),
        };
    }

    public void Update(UpdateCompanyRequest request)
    {
        Address = request.CompanyRequest.Address;
        AddressUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.Address);
        DirectorName = request.CompanyRequest.DirectorName;
        DirectorNameUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.DirectorName);
        DirectorPhone = request.CompanyRequest.DirectorPhone;
        Email = request.CompanyRequest.Email;
        CompanyType = request.CompanyRequest.CompanyType;
        CompanyTypeUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.CompanyType);
        Name = request.CompanyRequest.Name;
        NameUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.Name);
    }
}
