using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Files.UploadFiles;
using System.Net.Http.Headers;

namespace Application.UserCases.Commands.Files.UploadFiles;

internal sealed class UploadFilesCommandHandler(ICloudStorage _cloudStorage)
    : ICommandHandler<UploadFilesCommand>
{
    public async Task<Result.Success> Handle(UploadFilesCommand request, CancellationToken cancellationToken)
    {
        foreach(var file in request.ReceivedFiles)
        {
            var postedFileName = ContentDispositionHeaderValue
            .Parse(file.ContentDisposition)
            .FileName.Trim('"');
            await _cloudStorage.UploadFileAsync(file, postedFileName);
        }

        return Result.Success.Create();
    }
}
