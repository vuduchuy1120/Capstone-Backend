using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Files.GetFile;

namespace Application.UserCases.Queries.Files.GetFile;

internal sealed class GetFileQueryHandler(ICloudStorage _cloudStorage)
    : IQueryHandler<GetFileQuery, string>
{
    public async Task<Result.Success<string>> Handle(GetFileQuery request, CancellationToken cancellationToken)
    {
        var fileUrl = await _cloudStorage.GetSignedUrlAsync(request.fileName);

        return Result.Success<string>.Get(fileUrl);
    }
}

