using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.Phases;

public class PhaseNotFoundException : MyException
{
    public PhaseNotFoundException(Guid id)
       : base(400, $"Không tìm thấy giai đoạn sản phầm có id: {id}")
    {
    }

    public PhaseNotFoundException()
        : base(400, $"Không tìm thấy giai đoạn sản phẩm")
    {
    }
}
