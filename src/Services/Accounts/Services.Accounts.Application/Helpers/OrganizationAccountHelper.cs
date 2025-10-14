using System.Globalization;
using System.Security.Cryptography;

namespace Services.Accounts.Application.Helpers;
public static class OrganizationAccountHelper
{
    public static string GenerateUserName(string email)
    {
        return email.Split('@')[0];   
    }

    public static string GeneratePassword(string organizationName)
    {
        string initials = string.Concat(
            organizationName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w => char.ToUpper(w[0], CultureInfo.InvariantCulture))
        );

        char[] symbols = { '@', '#', '$' };
        char symbol = symbols[RandomNumberGenerator.GetInt32(symbols.Length)];

        string letters = new string(Enumerable.Range(0, 4)
            .Select(_ => (char)('a' + RandomNumberGenerator.GetInt32(26))).ToArray());

        string digits = new string(Enumerable.Range(0, 4)
            .Select(_ => (char)('0' + RandomNumberGenerator.GetInt32(10))).ToArray());

        string password = $"{initials}{symbol}{letters}{digits}";
        return password;
    }
}
