using System.Reflection;

namespace Services.Orders.Infrastructure;
internal static class InfrastructureReference
{
    public static Assembly Assembly => typeof(InfrastructureReference).Assembly;
}
