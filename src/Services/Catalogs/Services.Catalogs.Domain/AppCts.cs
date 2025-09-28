using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

    // Contains the directory of the file to seed the Map data
    public static class SeederFilePaths
    {
        // Get the folder at runtime
        private static readonly string JsonFolderPath = Path.Combine("Data", "SeedersFiles");

        // Each json file path
        public static readonly string MapFilePath = Path.Combine(JsonFolderPath, "Data/SeederFiles/Map.json");
    }
}
