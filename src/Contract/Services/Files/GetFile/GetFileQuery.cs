using Contract.Abstractions.Messages;

namespace Contract.Services.Files.GetFile;

public record GetFileQuery(string fileName) : IQuery<string>;