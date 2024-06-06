using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Files.GetFile;

namespace Application.UserCases.Queries.Files.GetFile;

internal sealed class GetFileQueryHandler(IFileService _fileService)
    : IQueryHandler<GetFileQuery, byte[]>
{
    public async Task<Result.Success<byte[]>> Handle(GetFileQuery request, CancellationToken cancellationToken)
    {
        var fileBytes = await _fileService.GetFile(request.fileName);

        return Result.Success<byte[]>.Get(fileBytes);
    }
}
