using Application.Abstractions.Data;
using Domain.Roles;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Role>()
            .HasIndex(role => role.RoleName)
            .IsUnique(true);
    }
}
