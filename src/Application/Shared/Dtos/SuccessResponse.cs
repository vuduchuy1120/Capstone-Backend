namespace Application.Shared.Dtos;

public class SuccessResponse<T>
{
    public int Status { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
}

