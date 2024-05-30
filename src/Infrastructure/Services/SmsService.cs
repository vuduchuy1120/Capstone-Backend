using Application.Abstractions.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

public class SmsService : ISmsService
{
    private readonly string _apiKeySid;
    private readonly string _apiKeySecret;

    public SmsService(string apiKeySid, string apiKeySecret)
    {
        _apiKeySid = apiKeySid;
        _apiKeySecret = apiKeySecret;
    }


    public async Task SendSmsAsync(string from, string to, string text)
    {
        var sms = new[]
        {
                new
                {
                    from,
                    to,
                    text
                }
            };

        var accessToken = GetAccessToken();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("X-STRINGEE-AUTH", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(JsonSerializer.Serialize(new { sms }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.stringee.com/v1/sms", content);

            Console.WriteLine($"STATUS: {response.StatusCode}");
            Console.WriteLine($"HEADERS: {response.Headers}");

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"BODY: {responseBody}");
        }
    }

    private string GetAccessToken()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var exp = now + 3600;

        var header = new JwtHeader(new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiKeySecret)),
            SecurityAlgorithms.HmacSha256))
        {
            {"cty", "stringee-api;v=1"}
        };

        var payload = new JwtPayload
        {
            { "jti", $"{_apiKeySid}-{now}" },
            { "iss", _apiKeySid },
            { "exp", exp },
            { "rest_api", 1 }
        };

        var token = new JwtSecurityToken(header, payload);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
