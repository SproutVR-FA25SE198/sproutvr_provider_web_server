using System.Reflection;

namespace Services.Notifications.Infrastructure;
internal static class InfrastructureReference
{
    public static Assembly Assembly => typeof(InfrastructureReference).Assembly;
}
