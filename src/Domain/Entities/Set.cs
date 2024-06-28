using Contract.Services.Set.CreateSet;
using Contract.Services.Set.UpdateSet;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Set : EntityAuditBase<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string ImageUrl { get; private set; }
    public string Description { get; private set; }
    public List<SetProduct>? SetProducts { get; private set; }
    public List<OrderDetail>? OrderDetails { get; set; }

    public static Set Create(CreateSetCommand request)
    {
        var createSetRequest = request.CreateSetRequest;
        var set = new Set()
        {
            Id = Guid.NewGuid(),
            Code = createSetRequest.Code,
            Name = createSetRequest.Name,
            ImageUrl = createSetRequest.ImageUrl,
            Description = createSetRequest.Description,
            CreatedBy = request.CreatedBy,
            CreatedDate = DateTime.UtcNow,
        };    

        if(createSetRequest.SetProductsRequest is null)
        {
            return set;
        }

        var setProducts = createSetRequest.SetProductsRequest
            .Select(req => SetProduct.Create(set.Id, req.ProductId, req.Quantity))
            .ToList();
        set.SetProducts = setProducts;

        return set;
    }

    public void Update(UpdateSetRequest updateSetRequest, string updatedBy)
    {
        Code = updateSetRequest.Code;
        Name = updateSetRequest.Name;
        ImageUrl = updateSetRequest.ImageUrl;
        Description = updateSetRequest.Description;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
    }
}
