using System.Security.Cryptography;

namespace Services.Orders.Application.Helpers;
public static class OrderUtils
{
    public static int GenerateOrderCode()
    {

        long ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        int pid = Environment.ProcessId % 100; 
        int rand = RandomNumberGenerator.GetInt32(0, 1000); 
        long raw = ms % 1000000 * 100 + pid + rand;
        return (int)(raw % 90000000 + 10000000);
    }
}
