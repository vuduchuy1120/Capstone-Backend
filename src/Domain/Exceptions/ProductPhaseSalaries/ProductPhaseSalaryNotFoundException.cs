using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.ProductPhaseSalaries;

public class ProductPhaseSalaryNotFoundException : MyException
{
    public ProductPhaseSalaryNotFoundException(Guid id)
      : base(400, $"Can not found product phase salary has id: {id}")
    {
    }

    public ProductPhaseSalaryNotFoundException()
        : base(400, $"Search product phase salary not found")
    {
    }
}
