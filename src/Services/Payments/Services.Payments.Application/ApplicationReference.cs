using System.Reflection;

namespace Services.Payments.Application; 
internal static class ApplicationReference
{
    public static Assembly Assembly => typeof(ApplicationReference).Assembly;
}
