using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    public DbSet<Slot> Slots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Role>()
            .HasIndex(role => role.RoleName)
            .IsUnique(true);

        modelBuilder.Entity<Attendance>().ToTable("Attendances");

        modelBuilder.Entity<Attendance>()
            .HasKey(a => new { a.UserId, a.SlotId, a.Date });

        //Attendance has foreign key is User and Slot and primary key is UserId, SLotId, Date
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.User)
            .WithMany(u => u.Attendances)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Slot)
            .WithMany(s => s.Attendances)
            .HasForeignKey(a => a.SlotId);

        modelBuilder.Entity<Slot>().ToTable("Slots");

        modelBuilder.Entity<ProductUnit>()
            .HasKey(pu => new { pu.ProductId, pu.SubProductId });

        modelBuilder.Entity<ProductUnit>()
            .HasOne(pc => pc.Product)
            .WithMany(pc => pc.ProductUnits)
            .HasForeignKey(pc => pc.ProductId);

        modelBuilder.Entity<ProductUnit>()
            .HasOne(pc => pc.SubProduct)
            .WithMany(pc => pc.SubProductUnits)
            .HasForeignKey(pc => pc.SubProductId);

        modelBuilder.Entity<ProductPharse>()
            .HasKey(ph => new {ph.PharseId, ph.ProductId});

        modelBuilder.Entity<ProductPharse>()
            .HasOne(ph => ph.Pharse)
            .WithMany(ph => ph.ProductPharses)
            .HasForeignKey(pc => pc.PharseId);

        modelBuilder.Entity<ProductPharse>()
            .HasOne(ph => ph.Product)
            .WithMany(ph => ph.ProductPharses)
            .HasForeignKey(pc => pc.ProductId);
    }
}
