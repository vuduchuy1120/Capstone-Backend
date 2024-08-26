using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class TokenRepository : ITokenRepository
{
    private readonly AppDbContext _context;

    public TokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Token token)
    {
        _context.Tokens.Add(token);
    }

    public void Delete(Token token)
    {
        _context.Tokens.Remove(token);
    }

    public async Task<Token> GetByUserIdAsync(string userId)
    {
        return await _context.Tokens.SingleOrDefaultAsync(t => t.UserId == userId);
    }

    public void Update(Token token)
    {
        _context.Tokens.Update(token);
    }
}
