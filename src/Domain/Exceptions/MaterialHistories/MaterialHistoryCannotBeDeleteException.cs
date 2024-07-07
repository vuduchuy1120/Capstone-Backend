using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.MaterialHistories;

public class MaterialHistoryCannotBeDeleteException : MyException
{
    public MaterialHistoryCannotBeDeleteException()
        : base(400, $"Lịch sử nhập nguyên liệu này không thể xoá do số lượng có thể vận chuyển trong kho nhỏ hơn số lượng cần xoá")
    {
    }
}
