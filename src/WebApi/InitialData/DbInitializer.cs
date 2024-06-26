using Application.Abstractions.Services;
using Contract.Services.Company.Create;
using Contract.Services.Company.Shared;
using Contract.Services.Role.Create;
using Contract.Services.Slot.Create;
using Contract.Services.User.CreateUser;
using Domain.Entities;
using Persistence;

namespace WebApi.InitialData;

public class DbInitializer
{
    public static void InitDb(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            SeedData(scope.ServiceProvider);
        }
    }

    public static void SeedData(IServiceProvider provider)
    {
        var context = provider.GetService<AppDbContext>() as AppDbContext;
        if (!context.Roles.Any())
        {
            SeedRoleData(context);
        }

        if (!context.Companies.Any())
        {
            SeedCompanyData(context);
        }

        if (!context.Users.Any())
        {
            var passwordService = provider.GetService<IPasswordService>();
            SeedUserData(context, passwordService);
        }

        if (!context.Slots.Any())
        {
            SeedSlotData(context);
        }

    }

    public static void SeedRoleData(AppDbContext context)
    {
        var roles = new List<Role>
        {
            Role.Create(new CreateRoleCommand("MAIN_ADMIN", "HuyVu's father")),
            Role.Create(new CreateRoleCommand("BRAND_ADMIN", "HuyVu's father")),
            Role.Create(new CreateRoleCommand("COUNTER", "HuyVu's father")),
            Role.Create(new CreateRoleCommand("DRIVER", "HuyVu's father")),
            Role.Create(new CreateRoleCommand("USER", "HuyVu's father"))
        };

        context.Roles.AddRange(roles);
        context.SaveChanges();
    }

    public static void SeedSlotData(AppDbContext context)
    {
        var slots = new List<Slot>
        {
            Slot.Create(new CreateSlotCommand("Morning")),
            Slot.Create(new CreateSlotCommand("Afternoon")),
            Slot.Create(new CreateSlotCommand("OverTime"))
        };

        context.Slots.AddRange(slots);
        context.SaveChanges();
    }

    public static void SeedCompanyData(AppDbContext context)
    {
        var companies = new List<Company>
        {
            Company.Create(new CreateCompanyRequest(new Contract.Services.Company.ShareDto.CompanyRequest("Cơ sở chính", "Hà Nội", "Vũ Đức Huy",
            "0976099789", "admin@admin.com",CompanyType.FACTORY))),
            Company.Create(new CreateCompanyRequest(new Contract.Services.Company.ShareDto.CompanyRequest("Cơ sở phụ", "Hà Nội", "Vũ Đức Huy",
            "0976099789", "admin2@admin.com", CompanyType.FACTORY))),
            Company.Create(new CreateCompanyRequest(new Contract.Services.Company.ShareDto.CompanyRequest("Công ty đối tác sản xuất", "Hà Nội", "Vũ Đức Huy",
            "0976099789", "admin@admin.com", CompanyType.THIRD_PARTY_COMPANY))),
            Company.Create(new CreateCompanyRequest(new Contract.Services.Company.ShareDto.CompanyRequest("Công ty cổ phần ABC", "Hà Nội", "Vũ Đức Huy",
            "0976099789", "admin@admin.com", CompanyType.CUSTOMER_COMPANY))),
        };

        context.Companies.AddRange(companies);
        context.SaveChanges();
    }

    public static void SeedUserData(AppDbContext context, IPasswordService passwordService)
    {
        var adminRole = context.Roles.FirstOrDefault();
        var companyId = context.Companies.FirstOrDefault(c => c.CompanyType == CompanyType.FACTORY).Id;
        var userCreateRequest = new CreateUserRequest(
            "001201011091",
            "Son",
            "Nguyen",
            "0976099351",
            "Ha Noi",
            "12345",
            "Male",
            "10/03/2001",
            200000,
            companyId,
            adminRole.Id
            );
        var hash = passwordService.Hash(userCreateRequest.Password);
        var user = User.Create(userCreateRequest, hash, userCreateRequest.Id);

        context.Users.Add(user);
        context.SaveChanges();
    }

}
