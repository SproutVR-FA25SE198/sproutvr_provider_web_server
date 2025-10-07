using System.Reflection;

namespace Services.Payments.Infrastructure.Extensions;
internal static class InfrastructureReference
{
    public static Assembly Assembly => typeof(InfrastructureReference).Assembly;
}
