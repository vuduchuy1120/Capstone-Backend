using Contract.Services.Attendance.Query;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IAttendanceRepository
{
    void AddAttendance(Attendance attendance);
    void UpdateAttendance(Attendance attendance);
    Task<Attendance?> GetAttendanceByUserIdSlotIdAndDateAsync(string userId, int slotId, DateOnly date);
    Task<List<Attendance>> GetAttendanceByDate(DateOnly date);
    Task<List<Attendance>> GetAttendanceByDateAndSlotId(DateOnly date, int slotId);
    Task<(List<Attendance>?, int)> SearchAttendancesAsync(GetAttendancesQuery request);
}
