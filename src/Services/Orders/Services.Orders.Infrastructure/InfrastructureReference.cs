using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services.Orders.Infrastructure;
internal static class InfrastructureReference
{
    public static Assembly Assembly => typeof(InfrastructureReference).Assembly;
}
