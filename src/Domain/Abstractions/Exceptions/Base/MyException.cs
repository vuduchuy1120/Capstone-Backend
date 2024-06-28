namespace Domain.Abstractions.Exceptions.Base;

public class MyException : Exception
{
    public int StatusCode { get; private set; }
    public Object? Error { get; private set; }
    public MyException(int code, string message) : base(message)
    {
        StatusCode = code;
        Error = null;
    }

    public MyException(int code, string message, Object error) : base(message)
    {
        StatusCode = code;
        Error = error;
    }
}