using Contract.Abstractions.Messages;
using Microsoft.AspNetCore.Http;

namespace Contract.Services.Files.UploadFiles;

public record UploadFilesCommand(IFormFileCollection ReceivedFiles) : ICommand;
