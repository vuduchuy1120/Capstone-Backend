using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.Materials;

public class MaterialNotFoundException : MyException
{
    public MaterialNotFoundException(int id)
        : base(400, $"Không tìm thấy nguyên liệu có id: {id}")
    {
    }

    public MaterialNotFoundException()
        : base(400, $"Không tìm thấy nguyên liệu")
    {
    }
}