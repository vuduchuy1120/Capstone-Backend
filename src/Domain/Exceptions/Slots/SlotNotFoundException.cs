using Domain.Abstractions.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.Slots
{
    public class SlotNotFoundException : MyException
    {
        public SlotNotFoundException() : base(400, $"Search slot not found")
        {
        }
    }
}
