using Application.Abstractions.Data;
using Contract.Services.Material.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.UnitTests.Materials;

public class AddMaterialTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IMaterialRepository _materialRepository;

    public AddMaterialTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _materialRepository = new MaterialRepository(_context);
    }

    [Fact]
    public async Task AddMaterial_Success_ShouldAddNewMaterialToDb()
    {
        var CreateMaterialRequest = new CreateMaterialRequest
        (
            Name: "Material 1",
            Description: "Description 1",
            Unit: "Unit 1",
            QuantityPerUnit: 10,
            Image: "Image 1"
        );
        var material = Material.Create(CreateMaterialRequest);

        _materialRepository.AddMaterial(material);
        await _context.SaveChangesAsync();

        Assert.Single(_context.Materials);
        var savedMaterial = await _context.Materials.FirstOrDefaultAsync(m => m.Name == material.Name);
        Assert.NotNull(savedMaterial);
        Assert.Equal("Material 1", savedMaterial.Name);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
