using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.EmployeeProduct.ShareDto;

public record EmployeeProductResponse
(
    string ImageUrl,
    string ProductName,
    string PhaseName,
    int Quantity
    );