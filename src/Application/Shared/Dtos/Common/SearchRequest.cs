namespace Application.Shared.Dtos.Common;

public class SearchRequest
{
    public string SearchTerm { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 4;
}
