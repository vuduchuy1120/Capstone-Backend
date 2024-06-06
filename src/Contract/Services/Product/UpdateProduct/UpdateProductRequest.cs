namespace Contract.Services.Product.UpdateProduct;

public record UpdateProductRequest(
    Guid Id, 
    string Code,
    decimal Price,
    string Size,
    string Description,
    bool IsGroup,
    string Name,
    bool IsInProcessing,
    ActionAddRequest Add,
    ActionRemoveRequest Remove);


/*
productId : 2323,
add : {
    images : [
        {
            IsBluePrint: true,
            IsMainImage: true,
            imageURL: sdfsdfsdf
        },
        {
            IsBluePrint: true,
            IsMainImage: true,
            imageURL: sdfsdfsdf
        }
    ],
    productUnits : [
         {
            subProductId: 33232,
            quantityPerUnit: 4
         },
          {
            subProductId: 33232,
            quantityPerUnit: 4
         }
    ]
    
},
remove : {
    images: [324234, 32234],
    productUnits: [32234, 34234]
}

*/