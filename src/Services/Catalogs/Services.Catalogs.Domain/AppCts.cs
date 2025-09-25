using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Catalogs.Domain;

public static class AppCts
{
    // Error Messages
    public static class Errors
    {
        // Maps
        public static class Maps
        {
            public const string NotFound = "The Map is not found. Please try again.";
            public const string AlreadyExists = "The Map is already existed.";
        }
    }
}
