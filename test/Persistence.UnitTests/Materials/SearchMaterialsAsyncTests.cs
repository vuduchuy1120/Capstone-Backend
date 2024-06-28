using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.UnitTests.Materials;

public class SearchMaterialsAsyncTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly MaterialRepository _materialRepository;

    public SearchMaterialsAsyncTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _materialRepository = new MaterialRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

