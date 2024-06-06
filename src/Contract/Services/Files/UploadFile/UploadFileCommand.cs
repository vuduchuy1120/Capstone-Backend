using Contract.Abstractions.Messages;
using Microsoft.AspNetCore.Http;

namespace Contract.Services.Files.UploadFile;

public record UploadFileCommand(IFormFile file) : ICommand<UploadFileResponse>;


