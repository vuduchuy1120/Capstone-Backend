using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IAttendanceRepository
{
    void AddAttendance(Attendance attendance);
    void UpdateAttendance(Attendance attendance);

    //getAttendanceByUserIdSlotIdAndDate
    Task<Attendance?> GetAttendanceByUserIdSlotIdAndDateAsync(string userId, int slotId, DateOnly date);
}
