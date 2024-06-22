using Microsoft.AspNetCore.Http;

namespace Contract.Services.Files.UploadFiles;

public record FileRequest(string FileName, IFormFile File);