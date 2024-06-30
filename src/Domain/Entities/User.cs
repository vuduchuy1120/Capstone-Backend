using Contract.Services.User.BanUser;
using Contract.Services.User.CreateUser;
using Contract.Services.User.UpdateUser;
using Domain.Abstractions.Entities;
using Domain.Exceptions.Users;

namespace Domain.Entities;

public class User : EntityAuditBase<string>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public string Address { get; private set; }
    public string Password { get; private set; }
    public string Gender { get; private set; }
    public string? Avatar { get; private set; } 
    public DateOnly DOB { get; set; }
    public decimal SalaryByDay { get; private set; }
    public bool IsActive { get; private set; }
    public int RoleId { get; private set; }
    public Role Role { get; private set; }
    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; }
    public List<Attendance>? Attendances { get; private set; }
    public List<EmployeeProduct>? EmployeeProducts { get; private set; }
    public List<Report>? Reports { get; private set; }
    public List<Shipment>? Shipments { get; private set; }
    public List<ShipOrder>? ShipOrders { get; private set; }
    public List<SalaryHistory>? SalaryHistories { get; private set; }
    public List<SalaryPay>? SalaryPays { get; private set; }

    public static User Create(CreateUserRequest request, string hashPassword, string createdBy)
    {
        return new()
        {
            Id = request.Id,
            Phone = request.Phone,
            Address = request.Address,
            Password = hashPassword,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            UpdatedBy = createdBy,
            UpdatedDate = DateTime.UtcNow,
            RoleId = request.RoleId,
            IsActive = true,
            SalaryByDay = request.SalaryByDay,
            DOB = CovertStringToDateTimeOnly(request.DOB),
            LastName = request.LastName,
            FirstName = request.FirstName,
            Gender = request.Gender,
            CompanyId = request.CompanyId,
            Avatar = string.IsNullOrWhiteSpace(request.Avatar) ? "image_not_found.png" : request.Avatar,
        };
    }

    public void Update(UpdateUserRequest request, string updatedBy)
    {
        Phone = request.Phone;
        Address = request.Address;
        RoleId = request.RoleId;
        SalaryByDay = request.SalaryByDay;
        DOB = CovertStringToDateTimeOnly(request.DOB);
        LastName = request.LastName;
        FirstName = request.FirstName;
        Gender = request.Gender;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
        CompanyId = request.CompanyId;
        Avatar = string.IsNullOrWhiteSpace(request.Avatar) ? "image_not_found.png" : request.Avatar;
    }

    public void UpdateStatus(ChangeUserStatusCommand request)
    {
        IsActive = request.isActive;
        UpdatedBy = request.updatedBy;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdatePassword(string password)
    {
        Password = password;
    }

    private static DateOnly CovertStringToDateTimeOnly(string dateString)
    {
        string format = "dd/MM/yyyy";

        DateTime dateTime;
        if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out dateTime))
        {
            return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
        }
        else
        {
            throw new WrongFormatDobException();
        }
    }
}
