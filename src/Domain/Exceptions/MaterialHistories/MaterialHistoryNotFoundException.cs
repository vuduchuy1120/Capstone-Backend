using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.MaterialHistories;

public class MaterialHistoryNotFoundException : MyException
{
    public MaterialHistoryNotFoundException(Guid id)
        : base(400, $"Không tìm thấy lịch sử nhập nguyên liệu có id: {id}")
    {
    }

    public MaterialHistoryNotFoundException()
        : base(400, $"Không tìm thấy lịch sử nhập nguyên liệu")
    {
    }
}