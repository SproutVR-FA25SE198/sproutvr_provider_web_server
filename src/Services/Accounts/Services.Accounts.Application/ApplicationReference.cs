using System.Reflection;

namespace Services.Accounts.Application;
internal static class ApplicationReference
{
    public static Assembly Assembly => typeof(ApplicationReference).Assembly;
}

