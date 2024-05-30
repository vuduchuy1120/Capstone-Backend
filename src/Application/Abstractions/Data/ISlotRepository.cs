using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Data;

public interface ISlotRepository
{
    void AddSlot(Slot slot);
    Task<List<Slot>> GetAllSlotsAsync();
}
