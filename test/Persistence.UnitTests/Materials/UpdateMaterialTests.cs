using Application.Abstractions.Data;
using Contract.Services.Material.Create;
using Contract.Services.Material.Update;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.UnitTests.Materials;

public class UpdateMaterialTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IMaterialRepository _materialRepository;
    public UpdateMaterialTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _materialRepository = new MaterialRepository(_context);
    }

    [Fact]
    public async Task UpdateMaterial_Success_ShouldUpdateMaterialInDb()
    {
        var createMaterialRequest = new CreateMaterialRequest
        (
            Name: "Material 1",
            Description: "Description 1",
            Unit: "Unit 1",
            QuantityPerUnit: 10,
            Image: "Image 1"
        );
        var material = Material.Create(createMaterialRequest);

        _materialRepository.AddMaterial(material);
        await _context.SaveChangesAsync();

        var updateMaterialRequest = new UpdateMaterialRequest
        (
            Id: 1,
            Name: "Material 2",
            Description: "Description 2",
            Unit: "Unit 2",
            QuantityPerUnit: 20,
            Image: "Image 2"
        );
        material.Update(updateMaterialRequest);
        _materialRepository.UpdateMaterial(material);
        await _context.SaveChangesAsync();

        var savedMaterial = await _context.Materials.FirstOrDefaultAsync(m => m.Name == material.Name);
        Assert.NotNull(savedMaterial);
        Assert.Equal("Material 2", savedMaterial.Name);
        Assert.Equal("Description 2", savedMaterial.Description);
        Assert.Equal("Unit 2", savedMaterial.Unit);
        Assert.Equal(20, savedMaterial.QuantityPerUnit);
        Assert.Equal("Image 2", savedMaterial.Image);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
