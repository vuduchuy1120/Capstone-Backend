using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Attendance : EntityAuditBaseWithoutId
    {
        private Attendance() { }

        public int SlotId { get; set; }
        public string UserId { get; set; }
        public DateTime Date {  get; set; }
        public double OverTime { get; set; }
        public Slot Slot { get; set; }
        public User User { get; set; }
        public bool IsSalaryByProduct { get; set; } = false;

    }
}
