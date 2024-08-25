using Application.Abstractions.Services;
using Contract.Services.Company.Create;
using Contract.Services.Company.Shared;
using Contract.Services.Phase.Creates;
using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.SharedDto;
using Contract.Services.ProductPhase.Creates;
using Contract.Services.Role.Create;
using Contract.Services.SalaryHistory.Creates;
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
        var context = provider.GetService<AppDbContext>();
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

        if (!context.Products.Any())
        {
            SeedProductData(context);
        }

        if (!context.Phases.Any())
        {
            SeedPhaseData(context);
        }

        if (!context.ProductPhases.Any())
        {
            SeedProductPhaseData(context);
        }

    }

    public static void SeedProductPhaseData(AppDbContext context)
    {
        var productPhases = new List<ProductPhase>();

        var products = context.Products.ToList();
        var phases = context.Phases.ToList();
        var mainFactory = context.Companies.FirstOrDefault(c => c.CompanyType == CompanyType.FACTORY && c.Name == "Cơ sở chính");

        foreach (var product in products)
        {
            foreach (var phase in phases)
            {
                var productPhase = ProductPhase
                    .Create(new CreateProductPhaseRequest(product.Id, phase.Id, 10, 10, mainFactory.Id));
                productPhases.Add(productPhase);
            }
        }

        context.ProductPhases.AddRange(productPhases);
        context.SaveChanges();
    }

    public static void SeedPhaseData(AppDbContext context)
    {
        var phases = new List<Phase>
        {
            Phase.Create(new CreatePhaseRequest("PH_001", "Giai đoạn tạo khung")),
            Phase.Create(new CreatePhaseRequest("PH_002", "Giai đoạn gia công")),
            Phase.Create(new CreatePhaseRequest("PH_003", "Giai đoạn hoàn thiện đóng gói")),
        };

        context.Phases.AddRange(phases);
        context.SaveChanges();
    }

    public static void SeedProductData(AppDbContext context)
    {
        var products = new List<Product>
        {
            Product.Create(
                new CreateProductRequest(
                    Code: "PD001",
                    PriceFinished: 50.00m,
                    PricePhase1: 20.00m,
                    PricePhase2: 30.00m,
                    Size: "M",
                    Description: "First product",
                    Name: "Product 1",
                    ImageRequests: null
                ),
                createdBy: "001201011091"
            ),
            Product.Create(
                new CreateProductRequest(
                    Code: "PD002",
                    PriceFinished: 100.00m,
                    PricePhase1: 40.00m,
                    PricePhase2: 60.00m,
                    Size: "L",
                    Description: "Second product",
                    Name: "Product 2",
                    ImageRequests: null
                ),
                createdBy: "001201011091"
            ),
            Product.Create(
                new CreateProductRequest(
                    Code: "PD003",
                    PriceFinished: 100.00m,
                    PricePhase1: 40.00m,
                    PricePhase2: 60.00m,
                    Size: "L",
                    Description: "Third product",
                    Name: "Product 3",
                    ImageRequests: null
                ),
                createdBy: "001201011091"
            ),
            Product.Create(
                new CreateProductRequest(
                    Code: "PD004",
                    PriceFinished : 100.00m,
                    PricePhase1 : 40.00m,
                    PricePhase2 : 60.00m,
                    Size: "L",
                    Description: "Four product",
                    Name: "Product 4",
                    ImageRequests: null
                ),
                createdBy: "001201011091"
            ),
        };

        var images = new List<ProductImage>
        {
            ProductImage.Create(products[0].Id, new ImageRequest("image_01.png", false, true)),
            ProductImage.Create(products[0].Id, new ImageRequest("image_02.png", true, false)),
            ProductImage.Create(products[1].Id, new ImageRequest("image_03.png", false, true)),
            ProductImage.Create(products[1].Id, new ImageRequest("image_04.png", true, false)),
            ProductImage.Create(products[2].Id, new ImageRequest("image_03.png", false, true)),
            ProductImage.Create(products[2].Id, new ImageRequest("image_04.png", true, false)),
            ProductImage.Create(products[3].Id, new ImageRequest("image_03.png", false, true)),
            ProductImage.Create(products[3].Id, new ImageRequest("image_04.png", true, false)),
        };

        context.Products.AddRange(products);
        context.ProductImages.AddRange(images);
        context.SaveChanges();
    }

    public static void SeedRoleData(AppDbContext context)
    {
        var roles = new List<Role>
        {
            Role.Create(new CreateRoleCommand("MAIN_ADMIN", "Quản lý hệ thống")),
            Role.Create(new CreateRoleCommand("BRANCH_ADMIN", "Quản lý cơ sở")),
            Role.Create(new CreateRoleCommand("COUNTER", "Quản lý số lượng")),
            Role.Create(new CreateRoleCommand("DRIVER", "Nhân viên vận chuyển")),
            Role.Create(new CreateRoleCommand("USER", "Nhân viên thường"))
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
            "iamge",
            "0976099351",
            "Ha Noi",
            "Male",
            "10/03/2001",
            SalaryByDayRequest: new SalaryByDayRequest(100000, "01/01/2024"),
            SalaryOverTimeRequest: new SalaryOverTimeRequest(150000, "01/01/2024"),
            companyId,
            adminRole.Id
            );
        var hash = passwordService.Hash("Dihson103@");
        var user = User.Create(userCreateRequest, hash, userCreateRequest.Id);

        context.Users.Add(user);
        context.SaveChanges();
    }

}
