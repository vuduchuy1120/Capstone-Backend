using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.EmployeeProduct.Creates;

public record CreateEmployeeProductRequest
(
    string Date,
    int SlotId,
    Guid CompanyId,
    List<CreateQuantityProductRequest> CreateQuantityProducts
    );
