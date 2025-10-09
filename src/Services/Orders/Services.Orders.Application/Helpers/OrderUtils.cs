using System.Security.Cryptography;

namespace Services.Orders.Application.Helpers;
public static class OrderUtils
{
    public static long GenerateOrderCode()
    {
        long ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); 
        int rand = RandomNumberGenerator.GetInt32(0, 1000);
        long combined = ms % 1000000000L * 1000 + rand; 
        return combined % 9000000000L + 1000000000L; 
    }
}
