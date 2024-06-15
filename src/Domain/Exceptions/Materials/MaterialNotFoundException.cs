using Contract.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.Materials;

public class MaterialNotFoundException : MyException
{
    public MaterialNotFoundException(int id)
        : base(400, $"Can not found material has id: {id}")
    {
    }

    public MaterialNotFoundException()
        : base(400, $"Search material not found")
    {
    }
}