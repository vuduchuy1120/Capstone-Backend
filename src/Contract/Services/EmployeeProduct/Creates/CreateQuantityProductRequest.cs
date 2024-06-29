using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.EmployeeProduct.Creates;

public record CreateQuantityProductRequest
(
    Guid ProductId,
    Guid PhaseId,
    int Quantity,
    string UserId
    );
