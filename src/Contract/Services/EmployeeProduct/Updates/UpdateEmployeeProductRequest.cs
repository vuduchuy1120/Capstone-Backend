namespace Contract.Services.EmployeeProduct.Updates;

public record UpdateEmployeeProductRequest
(
    List<UpdateQuantityProductRequest> UpdateQuantityProductRequests
    );
