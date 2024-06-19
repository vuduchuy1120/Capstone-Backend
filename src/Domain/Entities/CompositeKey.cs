using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class CompositeKey
{
    public string Date { get; set; }
    public int SlotId { get; set; }
    public Guid ProductId { get; set; }
    public Guid PhaseId { get; set; }
    public string UserId { get; set; }
}
