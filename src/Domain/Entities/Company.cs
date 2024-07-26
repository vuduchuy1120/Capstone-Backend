using Contract.Abstractions.Shared.Utils;
using Contract.Services.Company.Create;
using Contract.Services.Company.Shared;
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
    public List<Order>? Orders { get; set; }
    public CompanyType CompanyType { get; set; }
    public List<User>? Users { get; set; }
    public List<ProductPhase>? ProductPhases { get; set; }
    public List<MonthlyCompanySalary>? MonthlyCompanySalaries { get; set; }

    public static Company Create(CreateCompanyRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Address = request.CompanyRequest.Address.Trim(),
            AddressUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.Address.Trim()),
            DirectorName = request.CompanyRequest.DirectorName.Trim(),
            DirectorNameUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.DirectorName.Trim()),
            DirectorPhone = request.CompanyRequest.DirectorPhone.Trim(),
            Email = request.CompanyRequest.Email.Trim(),
            CompanyType = request.CompanyRequest.CompanyType,
            Name = request.CompanyRequest.Name.Trim(),
            NameUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.Name.Trim()),
        };
    }

    public void Update(UpdateCompanyRequest request)
    {
        Address = request.CompanyRequest.Address.Trim();
        AddressUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.Address.Trim());
        DirectorName = request.CompanyRequest.DirectorName.Trim();
        DirectorNameUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.DirectorName.Trim());
        DirectorPhone = request.CompanyRequest.DirectorPhone;
        Email = request.CompanyRequest.Email.Trim();
        CompanyType = request.CompanyRequest.CompanyType;
        Name = request.CompanyRequest.Name.Trim();
        NameUnAccent = StringUtils.RemoveDiacritics(request.CompanyRequest.Name.Trim());
    }
}
