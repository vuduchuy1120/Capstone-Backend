using Contract.Services.ProductPhase.Queries;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IProductPhaseRepository
{
    void AddProductPhase(ProductPhase productPhase);
    void UpdateProductPhase(ProductPhase productPhase);
    Task<ProductPhase> GetProductPhaseByPhaseIdAndProductId(Guid productId, Guid phaseId);
    Task<List<ProductPhase>> GetProductPhasesByProductId(Guid productId);
    Task<List<ProductPhase>> GetProductPhasesByPhaseId(Guid phaseId);
    void DeleteProductPhase(ProductPhase productPhase);
    Task<bool> IsProductPhaseExist(Guid productId, Guid phaseId);
    Task<(List<ProductPhase>?, int)> GetProductPhases(GetProductPhasesQuery getProductPhasesQuery);
    void AddProductPhaseRange(List<ProductPhase> productPhases);
    void UpdateProductPhaseRange(List<ProductPhase> productPhases);
    Task<ProductPhase> GetByProductIdPhaseIdCompanyID(Guid productId, Guid phaseId,Guid mainCompanyID);
    //Task<List<ProductPhase>> GetByProductIdPhaseIdCompanyID(Guid companyId);

}
