using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Files.UploadFile;
using Domain.Exceptions.Files;
using MediatR;

namespace Application.UserCases.Commands.Files.UploadFile;

internal sealed class UploadFileCommandHanlder(IFileService _fileService)
    : ICommandHandler<UploadFileCommand, UploadFileResponse>
{
    public async Task<Result.Success<UploadFileResponse>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var fileName = await _fileService.Upload(request.file);

        var response = new UploadFileResponse(fileName);
        return Result.Success<UploadFileResponse>.Upload(response);
    }
}
