using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Services;

public interface ICloudStorage
{
    Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage);
    Task DeleteFileAsync(string fileNameForStorage);
    Task<string> GetSignedUrlAsync(string fileNameForStorage);
}

