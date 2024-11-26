using Domain.Entities;

namespace Application.Abstractions.Data;

public interface ITokenRepository
{
    Task<Token> GetByUserIdAsync(string userId);
    void Add(Token token);
    void Delete(Token token);
    void Update(Token token);
}
