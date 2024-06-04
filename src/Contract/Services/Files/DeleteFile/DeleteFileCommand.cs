using Contract.Abstractions.Messages;

namespace Contract.Services.Files.DeleteFile;

public record DeleteFileCommand(string FileName) : ICommand;