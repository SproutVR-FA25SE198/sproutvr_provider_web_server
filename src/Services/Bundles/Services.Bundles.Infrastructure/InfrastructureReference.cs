using System.Reflection;

namespace Services.Bundles.Infrastructure;
public static class InfrastructureReference
{
    public static Assembly Assembly => typeof(InfrastructureReference).Assembly;
}
