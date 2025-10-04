using System.Reflection;
namespace Services.Baskets.Application;
internal static class ApplicationReference
{
    public static Assembly Assembly => typeof(ApplicationReference).Assembly;
}
