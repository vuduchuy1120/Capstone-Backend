namespace Contract.Services.EmployeeProduct.Deletes;

public record DeleteEmployeeProductRequest
    (
        List<DeleteQuantityProductRequest> DeleteQuantityProductRequests
    );
