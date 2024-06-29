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
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Set> Sets { get; set; }
    public DbSet<SetProduct> SetProducts { get; set; }
    public DbSet<Phase> Phases { get; set; }
    public DbSet<ProductPhase> ProductPhases { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<MaterialHistory> MaterialHistories { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<ShipOrder> ShipOrders { get; set; }
    public DbSet<ShipmentDetail> ShipmentDetails { get; set; }
    public DbSet<ShipOrderDetail> ShipOrderDetails { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<EmployeeProduct> EmployeeProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Phone)
            .IsUnique(true);

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

        modelBuilder.Entity<Material>().ToTable("Materials");

        modelBuilder.Entity<MaterialHistory>().ToTable("MaterialHistories");

        modelBuilder.Entity<MaterialHistory>()
            .HasOne(m => m.Material)
            .WithMany(m => m.MaterialHistories)
            .HasForeignKey(m => m.MaterialId);
        modelBuilder.Entity<MaterialHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<SetProduct>()
            .HasKey(pu => new { pu.ProductId, pu.SetId });

        modelBuilder.Entity<SetProduct>()
            .HasOne(sp => sp.Product)
            .WithMany(p => p.SetProducts)
            .HasForeignKey(sp => sp.ProductId);

        modelBuilder.Entity<SetProduct>()
            .HasOne(sp => sp.Set)
            .WithMany(p => p.SetProducts)
            .HasForeignKey(sp => sp.SetId);

        modelBuilder.Entity<ProductPhase>()
            .HasKey(ph => new { ph.PhaseId, ph.ProductId, ph.CompanyId });

        modelBuilder.Entity<ProductPhase>()
            .HasOne(ph => ph.Phase)
            .WithMany(ph => ph.ProductPhases)
            .HasForeignKey(pc => pc.PhaseId);

        modelBuilder.Entity<ProductPhase>()
            .HasOne(ph => ph.Product)
            .WithMany(ph => ph.ProductPhases)
            .HasForeignKey(pc => pc.ProductId);

        modelBuilder.Entity<ProductPhase>()
            .HasOne(ph => ph.Company)
            .WithMany(ph => ph.ProductPhases)
            .HasForeignKey(pc => pc.CompanyId);

        modelBuilder.Entity<EmployeeProduct>().ToTable("EmployeeProducts");

        modelBuilder.Entity<EmployeeProduct>()
            .HasKey(ep => new { ep.ProductId, ep.UserId, ep.PhaseId, ep.SlotId, ep.Date });

        modelBuilder.Entity<EmployeeProduct>()
            .HasOne(ep => ep.Product)
            .WithMany(p => p.EmployeeProducts)
            .HasForeignKey(ep => ep.ProductId);

        modelBuilder.Entity<EmployeeProduct>()
            .HasOne(ep => ep.User)
            .WithMany(u => u.EmployeeProducts)
            .HasForeignKey(ep => ep.UserId);

        modelBuilder.Entity<EmployeeProduct>()
            .HasOne(ep => ep.Phase)
            .WithMany(p => p.EmployeeProducts)
            .HasForeignKey(ep => ep.PhaseId);

        modelBuilder.Entity<EmployeeProduct>()
            .HasOne(ep => ep.Slot)
            .WithMany(s => s.EmployeeProducts)
            .HasForeignKey(ep => ep.SlotId);

        modelBuilder.Entity<Company>().ToTable("Companies");
        modelBuilder.Entity<Order>().ToTable("Orders");
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Company)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CompanyId);

        modelBuilder.Entity<Shipment>()
           .HasOne(s => s.FromCompany)
           .WithMany()
           .HasForeignKey(s => s.FromId)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Shipment>()
            .HasOne(s => s.ToCompany)
            .WithMany()
            .HasForeignKey(s => s.ToId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
