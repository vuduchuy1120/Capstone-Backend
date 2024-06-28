using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Files.DeleteFile;

namespace Application.UserCases.Commands.Files.DeleteFile;

internal sealed class DeleteFileCommandHandler(IFileService _fileService)
    : ICommandHandler<DeleteFileCommand>
{
    public Task<Result.Success> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            throw new Domain.Exceptions.Files.FileNotFoundException();
        }

        _fileService.Delete(request.FileName);

        return Task.FromResult(Result.Success.Delete());
    }
}
