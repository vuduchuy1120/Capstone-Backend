using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Material.ShareDto;

public record MaterialResponse
(
    int Id,
    string Name,
    string? Description,
    string Unit,
    double? QuantityPerUnit,
    string? Image
    );
