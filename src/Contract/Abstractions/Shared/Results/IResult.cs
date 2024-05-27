namespace Contract.Abstractions.Shared.Results;

public abstract class IResult
{
    public int status { get; set; }
    public string message { get; set; }
    public bool isSuccess { get; set; }
}
