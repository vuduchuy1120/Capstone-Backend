namespace Domain.Abstractions.Entities.Base;

internal interface IDateTracking
{
    DateTime CreatedDate { get; set; }
    DateTime UpdatedDate { get; set; }
}
