using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Files.UploadFile;

namespace Application.UserCases.Commands.Files.UploadFile;

internal sealed class UploadFileCommandHanlder(ICloudStorage _cloudStorage)
    : ICommandHandler<UploadFileCommand, UploadFileResponse>
{
    public async Task<Result.Success<UploadFileResponse>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var fileName = await _cloudStorage.UploadFileAsync(request.file, request.fileName);

        var response = new UploadFileResponse(fileName);
        return Result.Success<UploadFileResponse>.Upload(response);
    }
}
