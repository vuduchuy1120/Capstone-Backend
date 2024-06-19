namespace Contract.Services.EmployeeProduct.Updates;

public record UpdateQuantityProductRequest
(
    string Date,
    int SlotId,
    Guid ProductId,
    Guid PhaseId,
    int Quantity,
    string UserId,
    bool IsMold
    );