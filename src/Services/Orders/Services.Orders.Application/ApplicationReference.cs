using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services.Orders.Application;
internal static class ApplicationReference
{
    public static Assembly Assembly => typeof(ApplicationReference).Assembly;
}
