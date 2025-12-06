using System.Security.Cryptography;
using Services.Orders.Application.Abstractions.Services;
using Services.Orders.Application.Helpers;

namespace Services.Orders.Infrastructure.Services;
public class ActivationKeyGeneratorService : IActivationKeyGeneratorService
{
    public ActivationKeyGeneratorService() { }

    /// <summary>
    /// Generates a random, human-readable activation key.
    /// </summary>
    /// <returns>A random key (e.g., "A1B2C-D3E4F-G5H6J").</returns>
    public string Generate()
    {
        // For a 25-character key (5 groups of 5), we need 16 bytes.
        // 16 bytes * 8 bits/byte = 128 bits
        // 128 bits / 5 bits/char = 25.6 chars (Base32 will be 26 chars)
        byte[] randomBytes = RandomNumberGenerator.GetBytes(16);

        string keyString = Base32EncodingUtils.ToBase32String(randomBytes);

        // Trim to 25 chars and format
        return FormatKey(keyString.Substring(0, 25));
    }

    private static string FormatKey(string key)
    {
        // Inserts a dash every 5 characters
        return string.Join("-", Enumerable.Range(0, 5)
            .Select(i => key.Substring(i * 5, 5)));
    }
}
