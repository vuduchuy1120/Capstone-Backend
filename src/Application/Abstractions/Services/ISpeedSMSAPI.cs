
namespace Application.Abstractions.Services;

public interface ISpeedSMSAPI
{
    String sendSMS(String[] phones, String content, int type);
}
