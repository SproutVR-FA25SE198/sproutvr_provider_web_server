using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Catalogs.Domain;

public static class AppCts
{
    // Seeding File Jsons
    public static class SeederFiles
    {
        public const string Maps = "maps.json";
    }


    // Error Messages
    public static class Errors
    {
        // Maps
        public const string NotFound = "The Map is not found. Please try again.";
        public const string AlreadyExists = "The Map is already existed.";
    }

}
