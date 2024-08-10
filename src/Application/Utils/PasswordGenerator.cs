using System.Text;

namespace Application.Utils;

public class PasswordGenerator
{
    public static string GenerateRandomPassword(int length = 10)
    {
        if (length < 6) throw new ArgumentException("Password must be at least 6 characters long.");

        const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string specialChars = "!@#$%&*?";
        const string allChars = lowerCase + upperCase + digits + specialChars;

        var random = new Random();
        var password = new StringBuilder();

        password.Append(lowerCase[random.Next(lowerCase.Length)]);
        password.Append(upperCase[random.Next(upperCase.Length)]);
        password.Append(digits[random.Next(digits.Length)]);
        password.Append(specialChars[random.Next(specialChars.Length)]);

        for (int i = 4; i < length; i++)
        {
            password.Append(allChars[random.Next(allChars.Length)]);
        }

        return new string(password.ToString().OrderBy(_ => random.Next()).ToArray());
    }
}
