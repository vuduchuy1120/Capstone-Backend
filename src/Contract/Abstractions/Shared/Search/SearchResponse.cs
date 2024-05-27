namespace Contract.Abstractions.Shared.Search;

public record SearchResponse<T>(int CurrentPage, int TotalPages, T Data);
