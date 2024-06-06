namespace Contract.Services.Product.UpdateProduct;

public record AddProductImageRequest(string ImageUrl, bool IsBluePrint, bool IsMainImage);