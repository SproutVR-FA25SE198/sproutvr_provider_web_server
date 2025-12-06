using System.Reflection;

namespace Services.Orders.Application;
internal static class ApplicationReference
{
    public static Assembly Assembly => typeof(ApplicationReference).Assembly;
}
