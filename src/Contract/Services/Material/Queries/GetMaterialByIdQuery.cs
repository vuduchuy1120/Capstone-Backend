using Contract.Abstractions.Messages;
using Contract.Services.Material.ShareDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Material.Query;

public record GetMaterialByIdQuery(int Id) : IQuery<MaterialResponse>;
