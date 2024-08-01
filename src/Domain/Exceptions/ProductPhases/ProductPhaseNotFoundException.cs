using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.ProductPhases;


public class ProductPhaseNotFoundException : MyException
{
    public ProductPhaseNotFoundException(Guid id)
       : base(400, $"Không tìm thấy sản phầm có id: {id}")
    {
    }

    public ProductPhaseNotFoundException()
        : base(400, $"Không tìm thấy sản phẩm trong kho")
    {
    }
}

