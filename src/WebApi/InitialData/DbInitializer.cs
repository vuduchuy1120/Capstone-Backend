using Application.Abstractions.Services;
using Contract.Services.Role.Create;
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

        if(!context.Users.Any())
        {
            var passwordService = provider.GetService<IPasswordService>();
            SeedUserData(context, passwordService);
        }
    }

    public static void SeedRoleData(AppDbContext context)
    {
        var roles = new List<Role>
        {
            Role.Create(new CreateRoleCommand("MAIN_ADMIN", "HuyVu's father")),
            Role.Create(new CreateRoleCommand("BRAND_ADMIN", "HuyVu's father")),
            Role.Create(new CreateRoleCommand("COUNTER", "HuyVu's father"))
        };

        context.Roles.AddRange(roles);
        context.SaveChanges();
    }

    public static void SeedUserData(AppDbContext context, IPasswordService passwordService)
    {
        var adminRole = context.Roles.FirstOrDefault();
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
            adminRole.Id
            );
        var hash = passwordService.Hash(userCreateRequest.Password);
        var user = User.Create(userCreateRequest, hash, userCreateRequest.Id);

        context.Users.Add(user);
        context.SaveChanges();
    }

}
