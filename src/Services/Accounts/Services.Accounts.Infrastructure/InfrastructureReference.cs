using System.Reflection;

namespace Services.Accounts.Infrastructure;
public static class InfrastructureReference
{
    public static Assembly Assembly => typeof(InfrastructureReference).Assembly;
}
