namespace Domain.Abstractions.Entities.Base;

internal interface IUserTracking
{
    string CreatedBy { get; set; }
    string? UpdatedBy { get; set; }
}
