using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.ProductPhases;


public class ProductPhaseNotFoundException : MyException
{
    public ProductPhaseNotFoundException(Guid id)
       : base(400, $"Can not found phase has id: {id}")
    {
    }

    public ProductPhaseNotFoundException()
        : base(400, $"Không tìm thấy sản phẩm trong kho")
    {
    }
}

