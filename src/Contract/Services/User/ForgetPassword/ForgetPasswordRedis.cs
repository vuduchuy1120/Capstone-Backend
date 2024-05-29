namespace Contract.Services.User.ForgetPassword;

public class ForgetPasswordRedis
{
    private ForgetPasswordRedis() { }

    public string UserId { get; set; }
    public string VerifyCode { get; set; }
    public int NumberUse { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpireDate { get; set; }

    public static ForgetPasswordRedis Create(string userId)
    {
        var random = new Random();
        const string digits = "0123456789";

        var verifyCode = new string(Enumerable.Repeat(digits, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());

        return new ForgetPasswordRedis
        {
            UserId = userId,
            VerifyCode = verifyCode,
            NumberUse = 0,
            CreatedDate = DateTime.Now,
            ExpireDate = DateTime.Now.AddMinutes(5)
        };
    }

    public bool IsVerifyCodeValid(string code)
    {
        if (NumberUse >= 3 || DateTime.Now > ExpireDate || VerifyCode != code)
        {
            return false;
        }

        return true;
    }

    public bool IsExpired()
    {
        var isExpire = DateTime.Now > ExpireDate;
        return DateTime.Now > ExpireDate;
    }

    public void UpdateVerifyCodeNumberUse()
    {
        NumberUse = NumberUse + 1;
    }
}
