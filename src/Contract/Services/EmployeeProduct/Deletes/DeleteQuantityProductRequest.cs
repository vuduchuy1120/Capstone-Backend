using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.EmployeeProduct.Deletes;

public record DeleteQuantityProductRequest
    (
        string Date,
        int SlotId,
        Guid ProductId,
        Guid PhaseId,
        string UserId
    );
