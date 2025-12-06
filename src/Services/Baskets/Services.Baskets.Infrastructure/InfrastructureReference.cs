using System.Reflection;

namespace Services.Baskets.Infrastructure;
internal static class InfrastructureReference
{
    public static Assembly Assembly => typeof(InfrastructureReference).Assembly;
}
