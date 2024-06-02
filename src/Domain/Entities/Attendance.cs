using Contract.Services.Attendance.Create;
using Contract.Services.Attendance.Update;
using Domain.Abstractions.Entities;
using Domain.Exceptions.Attendances;
using Domain.Exceptions.Users;

namespace Domain.Entities
{
    public class Attendance : EntityAuditBaseWithoutId
    {
        public int SlotId { get; set; }
        public string UserId { get; set; }
        public DateOnly Date { get; set; }
        public double HourOverTime { get; set; } = 0;
        public Slot? Slot { get; set; }
        public User? User { get; set; }
        public bool IsAttendance { get; set; } = false;
        public bool IsOverTime { get; set; } = false;
        public bool IsSalaryByProduct { get; set; } = false;

        public static Attendance Create(CreateAttendanceWithoutSlotIdRequest createAttendanceRequest,int slot, string CreatedBy)
        {
            return new Attendance()
            {
                SlotId = slot,
                UserId = createAttendanceRequest.UserId,
                Date = ConvertStringToDateTimeOnly(DateTime.UtcNow.Date.ToString("dd/MM/yyyy")),
                HourOverTime = createAttendanceRequest.HourOverTime,
                IsAttendance = createAttendanceRequest.IsAttendance,
                IsOverTime = createAttendanceRequest.IsOverTime,
                IsSalaryByProduct = createAttendanceRequest.IsSalaryByProduct,
                CreatedBy = CreatedBy,
                CreatedDate = DateTime.UtcNow
            };
        }

        public void Update(UpdateAttendanceWithoutSlotIdRequest updateAttendanceRequest, string updatedBy)
        {
            HourOverTime = updateAttendanceRequest.HourOverTime;
            IsAttendance = updateAttendanceRequest.IsAttendance;
            IsOverTime = updateAttendanceRequest.IsOverTime;
            IsSalaryByProduct = updateAttendanceRequest.IsSalaryByProduct;
            UpdatedBy = updatedBy;
            UpdatedDate = DateTime.UtcNow;            
        }

        private static DateOnly ConvertStringToDateTimeOnly(string dateString)
        {
            string format = "dd/MM/yyyy";

            DateTime dateTime;
            if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out dateTime))
            {
                return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
            }
            else
            {
                throw new WrongFormatDateException();
            }
        }
    }
}
