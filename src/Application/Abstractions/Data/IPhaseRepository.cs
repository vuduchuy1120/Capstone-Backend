using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Data;

public interface IPhaseRepository
{
    // isExistById
    Task<bool> IsExistById(Guid id);
    Task<bool> IsAllPhaseExistByIdAsync(List<Guid> phaseIds);

}
