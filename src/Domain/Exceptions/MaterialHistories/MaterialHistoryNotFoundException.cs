using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.MaterialHistories;

public class MaterialHistoryNotFoundException : MyException
{
    public MaterialHistoryNotFoundException(Guid id)
        : base(400, $"Can not found material has id: {id}")
    {
    }

    public MaterialHistoryNotFoundException()
        : base(400, $"Search material not found")
    {
    }
}