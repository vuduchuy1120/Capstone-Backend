namespace Application.Shared.Dtos.Common;

public class SearchResponse<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public T Data { get; set; }
}
