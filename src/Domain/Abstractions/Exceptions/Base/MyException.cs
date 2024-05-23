namespace Domain.Abstractions.Exceptions.Base;

public class MyException : Exception
{
    public int StatusCode { get; private set; }
    public MyException(int code, string message) : base(message)
    {
        StatusCode = code;
    }
}
