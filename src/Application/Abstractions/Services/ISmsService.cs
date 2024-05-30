namespace Application.Abstractions.Services;

public interface ISmsService
{
    Task SendSmsAsync(string from, string to, string text);
}
